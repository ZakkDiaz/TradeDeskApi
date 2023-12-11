using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradeDeskBroker.Models
{
    public struct MarketTick
    {
        public string Symbol { get; set; }
        public DateTime TradedOn { get; set; }
        public decimal Price { get; set; }
        public decimal Volume { get; set; }
        public int Id { get; set; }
        public bool IsBuy { get; set; }
    }
}
