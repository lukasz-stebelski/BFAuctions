using BFAuctions.Entities;
using BFAuctions.Processors;
using Grpc.Net.Client;
using Microsoft.Extensions.Configuration.Ini;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using Account = BFAuctions.Processors.Account;

namespace BFAuctions
{
    internal class MainDispatcher
    {
        private string MyAddress { get; set; }
        public MainDispatcher()
        {
            MyAddress = DotNetEnv.Env.GetString("NODE_ADDRESS");
        }

        public bool HandleCommand(string command)
        {
            string[] commandParts = command.Split('#');
            if (commandParts.Length < 1)
                return false;

            switch (commandParts[0].ToUpper())
            {
                case "A":
                    CreateAuction(commandParts[1], commandParts[2]);
                    break;
                case "B":
                    BidAuction(commandParts[1], commandParts[2]);
                    break;
                case "LAU":
                    ListAuctions();
                    break;
                case "LAC":
                    ListAccounts();
                    break;
                case "LB":
                    ListBids();
                    break;
                case "C":
                    CloseAuction(commandParts[1]);
                    break;
                default:
                    break;

            }
            return true;
        }


        public void CreateAuction(string name, string initialValue)
        {
            Console.WriteLine($"CreateAuction named {name} with initial value of {initialValue}...");
            var auctionAdded = DBService.AddAuction(new Entities.Auction() { item_name = name, price = double.Parse(initialValue) });
            BroadcastAuctionAdded(new Processors.Auction() { Id = auctionAdded.Id, ItemName = name, Price = double.Parse(initialValue) });
        }


        public void BidAuction(string name, string offer)
        {
            Console.WriteLine($"BidAuction {name} with {offer}");
            var bidAdded = DBService.RegisterNewBid(new Entities.Bid() { auction_name = name, offer = double.Parse(offer) });
            BroadcastBidAdded(new Processors.Bid() { Id = bidAdded.Id , AuctionName = name, Offer  = double.Parse(offer) });
        }

        public void CloseAuction(string name)
        {
            Console.WriteLine($"CloseAuction {name}");
            BroadcastAuctionClosed(new Processors.Auction() {   ItemName = name });
        }

        public void ListAuctions()
        {
            Console.WriteLine("ListAuctions...");
            var auctions = DBService.GetAllAuctions();
            foreach (Entities.Auction auction in auctions)
            {
                Console.WriteLine($"{auction.item_name} - {auction.price}");
            }
        }

        public void ListAccounts()
        {
            Console.WriteLine("ListAccounts...");
            var accounts = DBService.GetAllAccounts();
            foreach (Entities.Account account in accounts)
            {
                Console.WriteLine($"{account.account_name}");
            }
        }

        public void ListBids()
        {
            Console.WriteLine("ListBids...");
            var bids = DBService.GetAllBids();
            foreach (Entities.Bid bid in bids)
            {
                Console.WriteLine($"Bid for {bid.auction_name} of {bid.offer}");
            }
        }

        public void OnAuctionReceived()
        {
            Console.WriteLine("Received new Auction");
            this.ListAuctions();
            Console.WriteLine("*********************************");
            Console.WriteLine("");
        }

        public void OnAuctionClosed(string name)
        {
            Console.WriteLine($"Auction {name} Closed");
            this.ListAuctions();
            Console.WriteLine("*********************************");
            Console.WriteLine("");
        }

        public void OnBidReceived()
        {
            Console.WriteLine("Received new Bid. Auction Status:");
            this.ListAuctions();
            Console.WriteLine("*********************************");
            Console.WriteLine("");
        }

        public void BroadcastAuctionAdded(Processors.Auction au)
        {
            var knownPeers = DBService.GetAllAccounts();
            var nodeAddress = MyAddress;

            Parallel.ForEach(knownPeers, peer =>
            {
                if (!nodeAddress.Equals(peer.account_name))
                {

                    Console.WriteLine("BroadcastAuctionAdded to {0}", peer.account_name);
                    GrpcChannel channel = GrpcChannel.ForAddress(peer.account_name);
                    var accountExchange = new AuctionProcessor.AuctionProcessorClient(channel);
                    try
                    {
                        var response = accountExchange.Receive(au);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"{peer.account_name} failed: {ex.Message}");
                    }

                }
            });

        }

        public void BroadcastAuctionClosed(Processors.Auction au)
        {
            var knownPeers = DBService.GetAllAccounts();
            var nodeAddress = MyAddress;

            Parallel.ForEach(knownPeers, peer =>
            {
                if (!nodeAddress.Equals(peer.account_name))
                {

                    Console.WriteLine("BroadcastAuctionClosed to {0}", peer.account_name);
                    GrpcChannel channel = GrpcChannel.ForAddress(peer.account_name);
                    var accountExchange = new AuctionProcessor.AuctionProcessorClient(channel);
                    try
                    {
                        var response = accountExchange.Close(au);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"{peer.account_name} failed: {ex.Message}");
                    }

                }
            });

        }

        public void BroadcastBidAdded(Processors.Bid bi)
        {
            var knownPeers = DBService.GetAllAccounts();
            var nodeAddress = MyAddress;

            Parallel.ForEach(knownPeers, peer =>
            {
                if (!nodeAddress.Equals(peer.account_name))
                {

                    Console.WriteLine("BroadcastBidAdded to {0}", peer.account_name);
                    GrpcChannel channel = GrpcChannel.ForAddress(peer.account_name);
                    var accountExchange = new BidProcessor.BidProcessorClient(channel);
                    try
                    {
                        var response = accountExchange.Add(bi);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"{peer.account_name} failed: {ex.Message}");
                    }

                }
            });

        }

    }
}


