using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradeDeskData.Entities;

namespace TradeDeskData
{
    public interface IFinancialRepository
    {
        Task<IEnumerable<UserProfile>> GetUserProfilesAsync();
        Task<UserProfile> GetUserProfileByIdAsync(int id);
        Task<UserProfile> GetUserProfileByKeyAsync(string passkey);
        Task<int> CreateUserProfileAsync(UserProfile userProfile);
        Task<IEnumerable<TrackedSymbol>> GetTrackedSymbolsAsync();
        Task<IEnumerable<WatchedSymbol>> GetWatchedSymbolsByUserIdAsync(int userId);
        Task<int> CreateWalletAsync(Wallet wallet);
        Task<Wallet> GetWalletByUserIdAsync(int userId);
        Task<IEnumerable<Holding>> GetHoldingsByWalletIdAsync(int walletId);
        Task<int> CreateHoldingAsync(Holding holding);
        Task<int> AddFundsToUserWalletAsync(int userId, int walletId, decimal amountToAdd);
        Task<bool> AddWatchAsync(int userProfileId, int trackedSymbolId);
        Task<bool> RemoveWatchAsync(int userProfileId, int trackedSymbolId);
    }
}
