using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradeDeskBroker.Models
{
    public class Trade
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public decimal Margin { get; set; }
        public decimal Price { get; set; }
        public decimal Value { get; set; }
        public int Leverage { get; set; }
        public decimal StopLoss { get; set; }
        public decimal TakeProfit { get; set; }
        public string Symbol { get; set; }
        public decimal? ClosePrice { get; set; }
        public DateTime TradedOn { get; set; }
        public bool IsBuy { get; set; }
    }
}
