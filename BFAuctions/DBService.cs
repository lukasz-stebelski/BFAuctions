using BFAuctions.Entities;
using BFAuctions.Processors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;
using Account = BFAuctions.Entities.Account;
using Auction = BFAuctions.Entities.Auction;
using Bid = BFAuctions.Entities.Bid;

namespace BFAuctions
{
    public class DBService
    {

        public DBService()
        {

        }

        public static void InitializePeers()
        {
            using (DataContext ctx = new DataContext())
            {
                ctx.InitializePeers();
            }
        }

        public static List<Account> GetAllAccounts()
        {
            using (DataContext ctx = new DataContext())
            {
                return ctx.Accounts.ToList();
            }
        }

        public static List<Auction> GetAllAuctions()
        {
            using (DataContext ctx = new DataContext())
            {
                return ctx.Auctions.ToList();
            }
        }

        public static List<Bid> GetAllBids()
        {
            using (DataContext ctx = new DataContext())
            {
                return ctx.Bids.ToList();
            }
        }
        public static Bid RegisterNewBid(Bid bid)
        {
            using (DataContext ctx = new DataContext())
            {
                ctx.Bids.Add(bid);
                ctx.Auctions.FirstOrDefault(a => a.item_name == bid.auction_name);
                ctx.SaveChanges();
            }
            return bid;
        }

        
        public static Auction AddAuction(Auction auction)
        {
            using (DataContext ctx = new DataContext())
            {
                ctx.Auctions.Add(auction);
                ctx.SaveChanges();
            }
            return auction;
        }
    }
}