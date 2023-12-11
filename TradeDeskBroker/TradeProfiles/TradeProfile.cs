using TradeDeskBroker.Market;

namespace TradeDeskBroker
{
    public abstract class TradeProfile
    {
        // Common properties that all trade profiles might need
        public string Name { get; protected set; }

        // Constructor to set the profile name
        protected TradeProfile(string name)
        {
            Name = name;
        }

        // Abstract method to evaluate a trade signal. 
        // This must be implemented by all subclasses.
        public abstract Task<TradeSignal> EvaluateTradeSignalAsync(string symbol, IMarketService marketService, DateTime offset);
    }
}
