using TradeDeskBroker.Market;

public class ShortTermMovingAverage : Indicator
{

    public ShortTermMovingAverage(int periodSeconds) : base("Short Term Moving Average", periodSeconds)
    {
    }

    public override async Task<decimal> EvaluateCurrentValueAsync(string symbol, IMarketService marketService, DateTime offset)
    {
        DateTime endTime = offset;
        DateTime startTime = endTime.AddSeconds(-PeriodSeconds);

        var marketData = await marketService.GetDataInRange(symbol, startTime, endTime);

        return marketData.Average(tick => tick.Price);
    }

}
