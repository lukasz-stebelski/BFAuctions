using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BFAuctions.Entities
{
    public  class Auction
    {
        public int Id { get; set; }
        public string item_name { get; set; }
        public double price { get; set; }
    }
}
