using TradeDeskBroker.Models;
using TradeDeskData.Entities;

namespace TradeDeskBroker
{
    public interface ITradeContext
    {
        Task<Trade> SubmitTrade(TradeSignal signal);
        Task<IEnumerable<Trade>> GetTradesBetween(string symbol, DateTime start, DateTime end);
    }
}