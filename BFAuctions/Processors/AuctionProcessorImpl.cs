using BFAuctions.Processors;
using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace BFAuctions.Processors
{
    public class AuctionProcessorImpl : AuctionProcessor.AuctionProcessorBase
    {
        public override Task<Auction> Close(Auction request, ServerCallContext context)
        {
            MainDispatcher dispatcher = new MainDispatcher();
            dispatcher.OnAuctionClosed(request.ItemName);
            return Task.FromResult(request);
        }
        public override Task<Auction> Receive(Auction request, ServerCallContext context)
        {
            using (DataContext ctx = new DataContext())
            {
                ctx.Auctions.Add(new Entities.Auction() { item_name = request.ItemName, price = request.Price });
                ctx.SaveChanges();
            }
            MainDispatcher dispatcher = new MainDispatcher();
            dispatcher.OnAuctionReceived();
            return Task.FromResult(request);
        }

        public override Task<Auction> GetById(Auction request, ServerCallContext context)
        {
            using (DataContext ctx = new DataContext())
            {
                var result = ctx.Auctions.FirstOrDefault(a => a.Id == request.Id);
                Auction response = new Auction()
                {
                    Id = result.Id,
                    Price = result.price,
                    ItemName = result.item_name
                };
                return Task.FromResult(response);
            }

        }

        public override Task<AuctionList> GetByName(Auction request, ServerCallContext context)
        {
            using (DataContext ctx = new DataContext())
            {
                AuctionList finalResult = new AuctionList();
                var result = ctx.Auctions.Where(a => a.item_name == request.ItemName).ToList();
                foreach (var response in result)
                {
                    finalResult.Auctions.Add(new Auction()
                    {
                        Id = response.Id,
                        Price = response.price,
                        ItemName = response.item_name
                    });
                }
                return Task.FromResult(finalResult);
            }
        }

       

    }
}
