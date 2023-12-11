using TradeDeskBroker.Market;
using TradeDeskBroker.Models;

namespace TradeDeskBroker
{
    public class TradeStrategy
    {
        private readonly IMarketService _marketService;

        public TradeStrategy(IMarketService marketService)
        {
            _marketService = marketService;
        }

        public async Task<TradeSignal> EvaluateMarketAsync(TradeProfile tradeProfile, string symbol, DateTime from, DateTime to)
        {
            var marketData = await _marketService.GetDataInRange(symbol, from, to);

            foreach (var tick in marketData)
            {
                var signal = await tradeProfile.EvaluateTradeSignalAsync(symbol, _marketService, DateTime.UtcNow);

                if (signal != null)
                {
                    return signal; // Return the generated trade signal
                }
            }

            return null; // No signal generated
        }
    }
}
