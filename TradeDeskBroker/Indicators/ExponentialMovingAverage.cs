using TradeDeskBroker.Market;
using TradeDeskBroker.Models;

public class ExponentialMovingAverage : Indicator
{
    private readonly int _period;

    public ExponentialMovingAverage(int period) : base("ExponentialMovingAverage", period)
    {
        _period = period;
    }

    public override async Task<decimal> EvaluateCurrentValueAsync(string symbol, IMarketService marketService, DateTime offset)
    {
        // Calculate the start time for EMA calculation (considering _period)
        DateTime startTime = offset.AddSeconds(-_period);

        var marketData = await marketService.GetDataInRange(symbol, startTime, offset);

        return CalculateEMA(marketData);
    }

    private decimal CalculateEMA(IEnumerable<MarketTick> marketData)
    {
        var closingPrices = marketData.Select(tick => tick.Price).ToList();

        if (!closingPrices.Any())
        {
            return 0; // Or handle this scenario as appropriate
        }

        // Calculate the smoothing factor
        decimal smoothingFactor = 2m / (closingPrices.Count + 1);

        // Start with the first price as the initial EMA value
        decimal ema = closingPrices.First();

        // Apply the EMA formula to each subsequent price point
        foreach (var price in closingPrices.Skip(1))
        {
            ema = (price - ema) * smoothingFactor + ema;
        }

        return ema;
    }

}
