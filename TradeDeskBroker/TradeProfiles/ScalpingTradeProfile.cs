using TradeDeskBroker;
using TradeDeskBroker.Market;
using System;
using System.Threading.Tasks;

public class ScalpingTradeProfile : TradeProfile
{
    private readonly Indicator _veryShortTermEMA;
    private readonly Indicator _shortTermEMA;
    private readonly Indicator _vwap;
    private readonly Indicator _atr;
    private readonly Indicator _price;
    private readonly Indicator _pressureVelocity;
    private DateTime _lastSignalTime;

    // Default parameters for the indicators
    private const int veryShortTermPeriod = 60 * 5; // 5 periods for the very short-term EMA
    private const int shortTermPeriod = 60 * 15; // 15 periods for the short-term EMA
    private const int atrPeriod = 60 * 30; // 30 periods for ATR

    public ScalpingTradeProfile(IIndicatorFactory indicatorFactory) : base("ScalpingTradeProfile")
    {
        _veryShortTermEMA = indicatorFactory.CreateIndicator("ExponentialMovingAverage", veryShortTermPeriod);
        _shortTermEMA = indicatorFactory.CreateIndicator("ExponentialMovingAverage", shortTermPeriod);
        _vwap = indicatorFactory.CreateIndicator("VWAP", 0);
        _atr = indicatorFactory.CreateIndicator("AverageTrueRange", atrPeriod);
        _price = indicatorFactory.CreateIndicator("Price", 0);
        _pressureVelocity = indicatorFactory.CreateIndicator("RateOfChange", 300);
        _lastSignalTime = DateTime.MinValue;
    }

    public override async Task<TradeSignal> EvaluateTradeSignalAsync(string symbol, IMarketService marketService, DateTime offset)
    {
        if ((offset - _lastSignalTime).TotalSeconds < 15)
        {
            return null;
        }

        var indicatorTasks = new List<Task<decimal>>
        {
            _veryShortTermEMA.EvaluateCurrentValueAsync(symbol, marketService, offset),
            _shortTermEMA.EvaluateCurrentValueAsync(symbol, marketService, offset),
            _vwap.EvaluateCurrentValueAsync(symbol, marketService, offset),
            _atr.EvaluateCurrentValueAsync(symbol, marketService, offset),
            _price.EvaluateCurrentValueAsync(symbol, marketService, offset),
            _pressureVelocity.EvaluateCurrentValueAsync(symbol, marketService, offset)
        };

        var results = await Task.WhenAll(indicatorTasks);

        var vstEmaValue = results[0];
        var stEmaValue = results[1];
        var vwapValue = results[2];
        var atrValue = results[3];
        var price = results[4];
        var pressureVelocity = results[5];

        bool isBuySignal = vstEmaValue > stEmaValue && vstEmaValue > vwapValue;
        bool isSellSignal = vstEmaValue < stEmaValue && vstEmaValue < vwapValue;
        bool isInBounds = vstEmaValue > price && isBuySignal || vstEmaValue < price && isSellSignal;
        bool isPressureVelocityAligned = (isBuySignal && pressureVelocity > 0) || (isSellSignal && pressureVelocity < 0);

        if (isInBounds && isPressureVelocityAligned && (isBuySignal || isSellSignal))
        {
            _lastSignalTime = offset;

            return new TradeSignal
            {
                Symbol = symbol,
                IsBuy = isBuySignal,
                Price = price,
                SignalTime = offset,
                SignalWeight = (1 / atrValue),
                RiskLevel = 2m,
                Confidence = .5m,
            };
        }

        return null;
    }

}
