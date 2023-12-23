using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BFAuctions.Entities
{
    public  class Bid
    {
        public long Id { get; set; }
        public string auction_name { get; set; }
        public double offer { get; set; }
    }
}
