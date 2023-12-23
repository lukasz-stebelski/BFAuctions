using BFAuctions.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BFAuctions
{
    public class DataContext : DbContext
    {
        public DbSet<Account> Accounts{ get; set; }
        public DbSet<Bid> Bids { get; set; }
        public DbSet<Auction> Auctions { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=BFAuctions.db");
        }


        public void InitializePeers()
        {
            Database.EnsureCreated();
            var allEntities = Accounts.ToList();
            Accounts.RemoveRange(allEntities);
            SaveChanges();

            var entitiesToAdd = new[]
            {
                new Account { account_name = "http://127.0.0.1:5002"},
                new Account { account_name = "http://127.0.0.1:5023"},
                new Account { account_name = "http://127.0.0.1:5024"},
            };

            Accounts.AddRange(entitiesToAdd);
            int res = SaveChanges();

        }
    }
}
