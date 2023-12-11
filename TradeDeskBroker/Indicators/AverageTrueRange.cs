using TradeDeskBroker.Market;
using TradeDeskBroker.Models;
using System;
using System.Linq;
using System.Collections.Generic;

public class AverageTrueRange : Indicator
{
    private readonly int _period;

    public AverageTrueRange(int period) : base("AverageTrueRange", period)
    {
        _period = period;
    }

    public override async Task<decimal> EvaluateCurrentValueAsync(string symbol, IMarketService marketService, DateTime offset)
    {
        DateTime startTime = offset.AddSeconds(-_period); // Assuming data points are frequent (e.g., hourly or more frequent)
        var marketData = await marketService.GetDataInRange(symbol, startTime, offset);

        return CalculateATR(marketData);
    }

    private decimal CalculateATR(IEnumerable<MarketTick> marketData)
    {
        var priceDifferences = marketData
            .Zip(marketData.Skip(1), (previous, current) => Math.Abs(current.Price - previous.Price))
            .ToList();

        if (!priceDifferences.Any())
        {
            return 0; // No data to calculate ATR
        }

        // Calculate the initial ATR as the average of price differences
        decimal atr = priceDifferences.Average();

        // If there are more than one price differences, apply smoothing
        if (priceDifferences.Count > 1)
        {
            decimal smoothingFactor = 2m / (priceDifferences.Count + 1);
            foreach (var difference in priceDifferences)
            {
                atr = (difference - atr) * smoothingFactor + atr;
            }
        }

        return atr;
    }

}
