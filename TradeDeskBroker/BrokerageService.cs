using TradeDeskBroker.Models;
using TradeDeskData;
using TradeDeskData.Entities;

namespace TradeDeskBroker
{
    public class BrokerageService : IBrokerageService
    {
        private readonly IFinancialRepository _repo;
        public BrokerageService(IFinancialRepository repo)
        {
            _repo = repo;
        }

        public async Task SubmitTrade(Trade trade)
        {

        }

        public async Task<Wallet> GetWallet(int userId)
        {
            return await _repo.GetWalletByUserIdAsync(userId);
        }

        public async Task CreateWallet(int userId, bool isTest = true)
        {
            await _repo.CreateWalletAsync(new Wallet() { UserProfileId = userId, IsTest = isTest, Funds = 0 });
        }

        public async Task AddFunds(int userId, int walletId, decimal funds)
        {
            await _repo.AddFundsToUserWalletAsync(userId, walletId, funds);
        }

        public async Task<IEnumerable<TrackedSymbol>> GetTrackedSymbolsAsync()
        {
            return await _repo.GetTrackedSymbolsAsync();
        }

        public async Task<IEnumerable<WatchedSymbol>> GetWatchedSymbols(int userId)
        {
            return await _repo.GetWatchedSymbolsByUserIdAsync(userId);
        }

        public async Task RemoveWatch(int userId, int symbolId)
        {
            await _repo.RemoveWatchAsync(userId, symbolId);
        }

        public async Task AddWatch(int userId, int symbolId)
        {
            await _repo.AddWatchAsync(userId, symbolId);
        }
    }
}
