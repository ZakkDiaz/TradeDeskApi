using TradeDeskBroker.Market;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TradeDeskBroker
{
    internal class RateOfDensityChangeIndicator : Indicator
    {
        private const int IntervalDivision = 10; // Example division of the period into smaller intervals

        public RateOfDensityChangeIndicator(int periodSeconds) : base("RateOfDensityChange", periodSeconds)
        {
        }

        public override async Task<decimal> EvaluateCurrentValueAsync(string symbol, IMarketService marketService, DateTime offset)
        {
            DateTime startTime = offset.AddSeconds(-PeriodSeconds);
            var marketData = await marketService.GetDataInRange(symbol, startTime, offset);

            int intervalLength = PeriodSeconds / IntervalDivision;
            List<(DateTime intervalStart, int buyCount, int sellCount)> densityData = new List<(DateTime, int, int)>();

            for (int i = 0; i < IntervalDivision; i++)
            {
                DateTime intervalStart = offset.AddSeconds(-intervalLength * (IntervalDivision - i));
                DateTime intervalEnd = intervalStart.AddSeconds(intervalLength);

                var intervalData = marketData.Where(tick => tick.TradedOn >= intervalStart && tick.TradedOn < intervalEnd);
                int buyCount = intervalData.Count(tick => tick.IsBuy);
                int sellCount = intervalData.Count(tick => !tick.IsBuy);

                densityData.Add((intervalStart, buyCount, sellCount));
            }

            // Calculating velocity and acceleration
            decimal totalVelocity = 0m;
            decimal totalAcceleration = 0m;
            int previousVelocity = 0;

            for (int i = 1; i < densityData.Count; i++)
            {
                int currentDensityDifference = densityData[i].buyCount - densityData[i].sellCount;
                var previousDensityDifference = densityData[i - 1].buyCount - densityData[i - 1].sellCount;

                int velocity = currentDensityDifference - previousDensityDifference;
                totalVelocity += velocity;

                if (i > 1)
                {
                    int acceleration = velocity - previousVelocity;
                    totalAcceleration += acceleration;
                }

                previousVelocity = velocity;
            }

            // Example weighting - adjust as needed
            decimal weightingFactor = 1.5m;
            decimal combinedMetric = totalVelocity + (weightingFactor * totalAcceleration);

            return combinedMetric;
        }
    }
}
