using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradeDeskData.Entities
{
    public class DataStream
    {
        public int Id { get; set; } // Primary Key, Auto Increment
        public string Symbol { get; set; }
        public long EventTime { get; set; }
        public string EventType { get; set; }
        public int TradeType { get; set; } // 1 for buy, 2 for sell
        public decimal Price { get; set; }
        public decimal Quantity { get; set; }
        public long DealTime { get; set; }
    }
}
