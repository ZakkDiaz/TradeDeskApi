using Dapper;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using TradeDeskData.Entities;

namespace TradeDeskData
{
    public class FinancialRepository : IFinancialRepository
    {
        private readonly string _connectionString;

        public FinancialRepository(IOptions<DatabaseConfig> config)
        {
            _connectionString = config.Value.ConnectionString;
        }

        private async Task<T> ExecuteAsync<T>(Func<SqlConnection, Task<T>> func)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    var result = await func(connection);
                    return result;
                }
            }
            catch (Exception ex)
            {
                // Log exception here
                Console.WriteLine($"Error: {ex.Message}");
                return default;
            }
        }

        public Task<IEnumerable<UserProfile>> GetUserProfilesAsync()
        {
            return ExecuteAsync(conn => conn.QueryAsync<UserProfile>("SELECT * FROM UserProfiles"));
        }

        public Task<UserProfile> GetUserProfileByIdAsync(int id)
        {
            return ExecuteAsync(conn => conn.QueryFirstOrDefaultAsync<UserProfile>("SELECT * FROM UserProfiles WHERE Id = @Id", new { Id = id }));
        }

        public Task<UserProfile> GetUserProfileByKeyAsync(string passkey)
        {
            return ExecuteAsync(conn => conn.QueryFirstOrDefaultAsync<UserProfile>("SELECT * FROM UserProfiles WHERE Passkey = @Passkey", new { Passkey = passkey }));
        }

        public Task<int> CreateUserProfileAsync(UserProfile userProfile)
        {
            return ExecuteAsync(conn => conn.ExecuteAsync("INSERT INTO UserProfiles (Passkey, Name) VALUES (@Passkey, @Name)", userProfile));
        }

        public Task<IEnumerable<TrackedSymbol>> GetTrackedSymbolsAsync()
        {
            return ExecuteAsync(conn => conn.QueryAsync<TrackedSymbol>("SELECT * FROM TrackedSymbols"));
        }

        public Task<IEnumerable<WatchedSymbol>> GetWatchedSymbolsByUserIdAsync(int userId)
        {
            return ExecuteAsync(conn => conn.QueryAsync<WatchedSymbol>("SELECT * FROM WatchedSymbols WHERE UserProfileId = @UserId", new { UserId = userId }));
        }

        public Task<int> CreateWalletAsync(Wallet wallet)
        {
            return ExecuteAsync(conn => conn.ExecuteAsync("INSERT INTO Wallet (UserProfileId, Funds, IsTest) VALUES (@UserProfileId, @Funds, @IsTest)", wallet));
        }

        public Task<Wallet> GetWalletByUserIdAsync(int userId)
        {
            return ExecuteAsync(conn => conn.QueryFirstOrDefaultAsync<Wallet>("SELECT * FROM Wallet WHERE UserProfileId = @UserId", new { UserId = userId }));
        }

        public Task<IEnumerable<Holding>> GetHoldingsByWalletIdAsync(int walletId)
        {
            return ExecuteAsync(conn => conn.QueryAsync<Holding>("SELECT * FROM Holdings WHERE WalletId = @WalletId", new { WalletId = walletId }));
        }

        public Task<int> CreateHoldingAsync(Holding holding)
        {
            return ExecuteAsync(conn => conn.ExecuteAsync("INSERT INTO Holdings (WalletId, TrackedSymbolId, Amount) VALUES (@WalletId, @TrackedSymbolId, @Amount)", holding));
        }
        public async Task<int> AddFundsToUserWalletAsync(int userId, int walletId, decimal amountToAdd)
        {
            return await ExecuteAsync(async conn =>
            {
                string sql = "UPDATE Wallet SET Funds = Funds + @AmountToAdd WHERE Id = @WalletId AND UserProfileId = @UserId";
                return await conn.ExecuteAsync(sql, new { AmountToAdd = amountToAdd, WalletId = walletId, UserId = userId });
            });
        }


        public Task<bool> AddWatchAsync(int userProfileId, int trackedSymbolId)
        {
            return ExecuteAsync(async conn =>
            {
                const string sql = @"INSERT INTO WatchedSymbols (UserProfileId, TrackedSymbolId) VALUES (@UserProfileId, @TrackedSymbolId)";
                var affectedRows = await conn.ExecuteAsync(sql, new { UserProfileId = userProfileId, TrackedSymbolId = trackedSymbolId });
                return affectedRows > 0;
            });
        }

        public Task<bool> RemoveWatchAsync(int userProfileId, int trackedSymbolId)
        {
            return ExecuteAsync(async conn =>
            {
                const string sql = @"DELETE FROM WatchedSymbols WHERE UserProfileId = @UserProfileId AND TrackedSymbolId = @TrackedSymbolId";
                var affectedRows = await conn.ExecuteAsync(sql, new { UserProfileId = userProfileId, TrackedSymbolId = trackedSymbolId });
                return affectedRows > 0;
            });
        }
    }
}
