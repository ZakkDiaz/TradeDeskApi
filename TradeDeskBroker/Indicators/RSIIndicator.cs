using TradeDeskBroker.Market;
using TradeDeskBroker.Models;

public class RSIIndicator : Indicator
{

    public RSIIndicator(int periodSeconds) : base("RSI Indicator", periodSeconds)
    {
    }

    public override async Task<decimal> EvaluateCurrentValueAsync(string symbol, IMarketService marketService, DateTime offset)
    {
        DateTime endTime = offset;
        DateTime startTime = endTime.AddSeconds(-PeriodSeconds);

        var marketData = await marketService.GetDataInRange(symbol, startTime, endTime);

        return CalculateRSI(marketData, marketData.Count());
    }


    private decimal CalculateRSI(IEnumerable<MarketTick> marketData, int period)
    {
        var gains = new List<decimal>();
        var losses = new List<decimal>();

        // Assuming marketData is ordered by TradedOn
        var previousTick = marketData.First();

        foreach (var currentTick in marketData.Skip(1))
        {
            var change = currentTick.Price - previousTick.Price;

            if (change > 0)
            {
                gains.Add(change);
                losses.Add(0);
            }
            else
            {
                gains.Add(0);
                losses.Add(Math.Abs(change));
            }

            previousTick = currentTick;
        }

        decimal averageGain = gains.Take(period).Average();
        decimal averageLoss = losses.Take(period).Average();

        if (averageLoss == 0) return 100; // Prevent division by zero

        decimal rs = averageGain / averageLoss;
        decimal rsi = 100 - (100 / (1 + rs));

        return rsi;
    }
}