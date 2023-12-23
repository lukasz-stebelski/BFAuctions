using BFAuctions.Processors;
using Grpc.Core;
using Grpc.Net.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
 

namespace BFAuctions.Processors
{
    public class AccountProcessorImpl : AccountProcessor.AccountProcessorBase
    {
        public override Task<Account> Receive(Account request, ServerCallContext context)
        {
            using (DataContext ctx = new DataContext())
            {
                ctx.Accounts.Add(new Entities.Account() { account_name = request.AccountName });
                return Task.FromResult(request);
            }
        }


        public override Task<AccountList> GetAll(Empty request, ServerCallContext context)
        {
            return base.GetAll(request, context);
        }

    }
}
 