using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradeDeskData.Entities
{
    public class TradeSignalEntity
    {
        public int Id { get; set; }
        public string StrategyId { get; set; }
        public string Symbol { get; set; }
        public bool IsBuy { get; set; }
        public decimal SignalWeight { get; set; }
        public decimal Price { get; set; }
        public decimal RiskLevel { get; set; }
        public decimal Confidence { get; set; }
        public int UserId { get; set; }
        public int PeriodSeconds { get; set; }
        public int Leverage { get; set; }
        public DateTime SignalTime { get; set; }
        public decimal StopLoss { get; set; }
        public decimal TakeProfit { get; set; }
        public decimal CloseValue { get; set; }
        public bool Outcome { get; set; }
    }
}
