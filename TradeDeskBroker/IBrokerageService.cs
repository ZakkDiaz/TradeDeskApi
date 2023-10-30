using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradeDeskData.Entities;

namespace TradeDeskBroker
{
    public interface IBrokerageService
    {
        Task<Wallet> GetWallet(int userId);
        Task CreateWallet(int userId, bool isTest = true);
        Task AddFunds(int userId, int walletId, decimal funds);
        Task<IEnumerable<TrackedSymbol>> GetTrackedSymbolsAsync();
        Task<IEnumerable<WatchedSymbol>> GetWatchedSymbols(int userId);
        Task RemoveWatch(int userId, int symbolId);
        Task AddWatch(int userId, int symbolId);
    }
}
