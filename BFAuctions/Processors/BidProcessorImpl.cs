using BFAuctions.Processors;
using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
 

namespace BFAuctions.Processors
{
    public class BidProcessorImpl : BidProcessor.BidProcessorBase
    {
        public override Task<Bid> Add(Bid request, ServerCallContext context)
        {
            using (DataContext ctx = new DataContext())
            {
                ctx.Bids.Add(new Entities.Bid() { Id = request.Id, auction_name = request.AuctionName, offer = request.Offer});
                var auction = ctx.Auctions.FirstOrDefault(a => a.item_name == request.AuctionName);
                if (auction != null) 
                {
                    auction.price = request.Offer;
                }

                ctx.SaveChanges();
            }
            MainDispatcher dispatcher = new MainDispatcher();
            dispatcher.OnBidReceived();
            return Task.FromResult(request);
        }

        public override Task<Bid> GetById(Bid request, ServerCallContext context)
        {
            return base.GetById(request, context);
        }

        public override Task<BidList> GetAllBids(Bid request, ServerCallContext context)
        {
            return base.GetAllBids(request, context);
        }
    }
}
 