using TradeDeskBroker.Models;

namespace TradeDeskBroker
{
    public class TradeContext : ITradeContext
    {
        Dictionary<string, List<Trade>> _tradeDictionary = new Dictionary<string, List<Trade>>();
        private IBrokerageService _brokerageService;
        public TradeContext(IBrokerageService brokerageService)
        {
            _brokerageService = brokerageService;
        }

        public void UpsertTrade(Trade trade)
        {
            if (!_tradeDictionary.ContainsKey(trade.Symbol))
                _tradeDictionary.Add(trade.Symbol, new List<Trade>());
            _tradeDictionary[trade.Symbol].Add(trade);
        }

        public List<Trade> GetAllTradesForSymbol(string symbol)
        {
            if(_tradeDictionary.ContainsKey(symbol))
                return _tradeDictionary[symbol];
            return new List<Trade>();
        }

        public List<Trade> GetOpenTradesForSymbol(string symbol)
        {
            if (_tradeDictionary.ContainsKey(symbol))
                return _tradeDictionary[symbol].Where(t => t.ClosePrice == null).ToList();
            return new List<Trade>();
        }

        public List<Trade> GetClosedTradesForSymbol(string symbol)
        {
            if (_tradeDictionary.ContainsKey(symbol))
                return _tradeDictionary[symbol].Where(t => t.ClosePrice != null).ToList();
            return new List<Trade>();
        }


        public async Task<Trade> SubmitTrade(TradeSignal signal)
        {
            var wallet = await _brokerageService.GetWallet(signal.UserId);

            // Calculate margin as 10% of the wallet funds
            decimal margin = wallet.Funds * 0.1m;

            // Adjust leverage based on period and risk level
            var leverage = CalculateLeverage(signal.RiskLevel, signal.PeriodSeconds);

            // Calculate stop loss as 10% of the margin in negative PNL
            decimal stopLoss = signal.StopLoss != 0 ? signal.StopLoss : signal.Price - ((signal.SignalWeight / leverage) * (signal.IsBuy ? 1 : -1));

            // Calculate take profit based on a percentage of positive PNL
            decimal takeProfitFactor = CalculateTakeProfitFactor(signal.RiskLevel, signal.Confidence);
            decimal takeProfit = signal.TakeProfit != 0 ? signal.TakeProfit : signal.Price + (signal.RiskLevel * signal.SignalWeight * takeProfitFactor / leverage) * (signal.IsBuy ? 1 : -1);

            var trade = new Trade
            {
                Symbol = signal.Symbol,
                Price = signal.Price,
                Margin = margin,
                StopLoss = stopLoss,
                TakeProfit = takeProfit,
                Leverage = leverage,
                TradedOn = signal.SignalTime,
                IsBuy = signal.IsBuy
                // Set other properties as needed
            };

            UpsertTrade(trade);
            return trade;
        }

        private int CalculateLeverage(decimal riskLevel, int periodSeconds)
        {
            // Example logic: Lower leverage for longer periods, and adjust for risk level
            int baseLeverage = (periodSeconds < 3600) ? 100 : 25; // Less leverage for periods longer than an hour
            int adjustedLeverage = (int)(baseLeverage * (1 - riskLevel));
            return Math.Clamp(adjustedLeverage, 1, baseLeverage);
        }

        private decimal CalculateTakeProfitFactor(decimal riskLevel, decimal confidence)
        {
            return riskLevel * confidence;
        }
        public Task<IEnumerable<Trade>> GetTradesBetween(string symbol, DateTime start, DateTime end)
        {
            if (_tradeDictionary.TryGetValue(symbol, out var trades))
            {
                var filteredTrades = trades.Where(trade => trade.TradedOn >= start && trade.TradedOn <= end);
                return Task.FromResult<IEnumerable<Trade>>(filteredTrades.ToList());
            }

            return Task.FromResult<IEnumerable<Trade>>(new List<Trade>());
        }
    }
}