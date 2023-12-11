using TradeDeskBroker.Market;
using TradeDeskBroker.Models;

public class VWAPIndicator : Indicator
{
    public VWAPIndicator() : base("VWAP", 0)
    {
    }

public override async Task<decimal> EvaluateCurrentValueAsync(string symbol, IMarketService marketService, DateTime offset)
{
    DateTime endTime = offset; // NOW
    DateTime startTime = endTime.AddHours(-3); // 24 hours before NOW

    var marketData = await marketService.GetDataInRange(symbol, startTime, endTime);

    return CalculateVWap(marketData);
}


    private decimal CalculateVWap(IEnumerable<MarketTick> marketData)
    {
        decimal totalVolume = 0;
        decimal vwapNumerator = 0;

        foreach (var tick in marketData)
        {
            totalVolume += tick.Volume;
            vwapNumerator += tick.Price * tick.Volume;
        }

        return totalVolume > 0 ? vwapNumerator / totalVolume : 0;
    }
    
}