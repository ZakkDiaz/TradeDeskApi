using TradeDeskBroker;
using TradeDeskBroker.Market;
public class HighRsiProfile : TradeProfile
{
    private readonly Indicator _shortTermMovingAverage;
    private readonly Indicator _shortTermMAForConfirmation;

    private readonly Indicator _rsiIndicator;
    private readonly List<decimal> _maValues; // List to track the last few MA values
    private readonly decimal _rsiUpperThreshold; // Upper RSI threshold for overbought conditions
    private readonly decimal _rsiLowerThreshold; // Lower RSI threshold for oversold conditions
    private DateTime _lastTradeSignalTime;

    public HighRsiProfile(IIndicatorFactory indicatorFactory) : base("TestTradeProfile")
    {
        // Example parameters (adjust as needed)
        int maPeriodSeconds = 60 * 30; // 60 minutes
        int rsiPeriodSeconds = 60 * 5; // 60 minutes
        _rsiUpperThreshold = 95; // Upper threshold for RSI
        _rsiLowerThreshold = 05; // Lower threshold for RSI

        _shortTermMovingAverage = indicatorFactory.CreateIndicator("ShortTermMovingAverage", maPeriodSeconds);
        _rsiIndicator = indicatorFactory.CreateIndicator("RSIIndicator", rsiPeriodSeconds);

        // Short-term MA for trade confirmation
        int stmaPeriodSeconds = 60 * 5; // 5 minutes
        _shortTermMAForConfirmation = indicatorFactory.CreateIndicator("ShortTermMovingAverage", stmaPeriodSeconds);

        _lastTradeSignalTime = DateTime.MinValue;
    }

    public override async Task<TradeSignal> EvaluateTradeSignalAsync(string symbol, IMarketService marketService, DateTime offset)
    {
        if ((offset - _lastTradeSignalTime).TotalMinutes < 5)
        {
            return null; // Do not generate a new signal if less than 5 minutes
        }
        decimal maValue = await _shortTermMovingAverage.EvaluateCurrentValueAsync(symbol, marketService, offset);
        decimal maConfirmationValue = await _shortTermMAForConfirmation.EvaluateCurrentValueAsync(symbol, marketService, offset);

        decimal rsiValue = await _rsiIndicator.EvaluateCurrentValueAsync(symbol, marketService, offset);

        var data = await marketService.GetDataInRange(symbol, offset.AddSeconds(-5), offset);

        // Buy signal criteria: RSI below lower threshold and upward moving average trend
        bool isBuySignal = rsiValue < _rsiLowerThreshold && maValue < maConfirmationValue;

        // Sell signal criteria: RSI above upper threshold and downward moving average trend
        bool isSellSignal = rsiValue > _rsiUpperThreshold && maValue > maConfirmationValue;
        bool isConfirmationPositive = false;

        if (isBuySignal)
        {
            // For a buy signal, confirm if the short-term MA is above the last price
            isConfirmationPositive = maConfirmationValue > data.LastOrDefault().Price;
        }
        else if (isSellSignal)
        {
            // For a sell signal, confirm if the short-term MA is below the last price
            isConfirmationPositive = maConfirmationValue < data.LastOrDefault().Price;
        }

        if (isConfirmationPositive)
        {
            _lastTradeSignalTime = offset; // Update the last trade signal time
            return new TradeSignal
            {
                Symbol = symbol,
                IsBuy = isBuySignal,
                Confidence = 1 - Math.Abs(50 - rsiValue) / 50, // Example confidence calculation
                Price = data.LastOrDefault().Price, // Price the trade should be placed at
                SignalWeight = 1, // Example signal weight
                SignalTime = offset,
                RiskLevel = 1
            };
        }

        return null; // No signal generated
    }
}

