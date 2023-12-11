namespace TradeDeskBroker
{
    public class IndicatorFactory : IIndicatorFactory
    {
        public Indicator CreateIndicator(string type, int periodSeconds)
        {
            switch (type)
            {
                case "ShortTermMovingAverage":
                    return new ShortTermMovingAverage(periodSeconds);
                case "RSIIndicator":
                    return new RSIIndicator(periodSeconds);
                case "VWAP":
                    return new VWAPIndicator();
                case "ExponentialMovingAverage":
                    return new ExponentialMovingAverage(periodSeconds);
                case "AverageTrueRange":
                    return new AverageTrueRange(periodSeconds);
                case "Price":
                    return new PriceIndicator();
                case "RateOfChange":
                    return new RateOfDensityChangeIndicator(periodSeconds);
                default:
                    throw new ArgumentException("Invalid indicator type");
            }
        }
    }
}
