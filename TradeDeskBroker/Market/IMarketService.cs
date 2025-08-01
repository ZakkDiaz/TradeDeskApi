using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradeDeskBroker.Models;
using TradeDeskData.Entities;

namespace TradeDeskBroker.Market
{
    public interface IMarketService
    {
        Task<IEnumerable<MarketTick>> GetDataInRange(string symbol, DateTime from, DateTime to);
        Task<IEnumerable<TradeSignal>> GetTradesInRange(string symbol, DateTime from, DateTime to);
    }
}
