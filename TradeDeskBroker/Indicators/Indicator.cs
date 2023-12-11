using TradeDeskBroker.Market;

public abstract class Indicator
{
    public string Name { get; set; }
    protected int PeriodSeconds { get; set; }

    protected Indicator(string name, int periodSeconds)
    {
        Name = name;
        PeriodSeconds = periodSeconds;
    }

    public abstract Task<decimal> EvaluateCurrentValueAsync(string symbol, IMarketService marketService, DateTime offset);
}
