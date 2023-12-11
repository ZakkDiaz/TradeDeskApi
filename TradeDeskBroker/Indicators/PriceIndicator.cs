using System.Linq;
using TradeDeskBroker.Market;
using TradeDeskBroker.Models;

namespace TradeDeskBroker
{
    public class PriceIndicator : Indicator
    {
        public PriceIndicator() : base("Price", 0)
        {
        }

        public override async Task<decimal> EvaluateCurrentValueAsync(string symbol, IMarketService marketService, DateTime offset)
        {
            DateTime startTime = offset.AddSeconds(-60);
            var marketData = await marketService.GetDataInRange(symbol, startTime, offset);

            return marketData.Last().Price;
        }
    }
}