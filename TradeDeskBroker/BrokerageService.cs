using TradeDeskBroker.Models;

namespace TradeDeskBroker
{
    public class BrokerageService : IBrokerageService
    {
        private readonly Dictionary<string, Wallet> _wallets = new Dictionary<string, Wallet>();

        public Wallet GetWallet(string name)
        {
            if (!_wallets.ContainsKey(name))
                CreateWallet(name);
            return _wallets[name];
        }

        private void CreateWallet(string name)
        {
            _wallets.TryAdd(name, new Wallet() { Name = name, IsTest = true, TotalFunds = 0 });
        }

        public void AddFunds(string name, decimal funds)
        {
            var wallet = GetWallet(name);
            wallet.TotalFunds += funds;
            _wallets[name] = wallet;
        }
    }
}
