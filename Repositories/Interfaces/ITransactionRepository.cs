using MonthlyExpenseTracker.Models;

namespace MonthlyExpenseTracker.Repositories.Interfaces
{
    public interface ITransactionRepository
    {
        Task<List<Transaction>> GetByUserIdAsync(string userId);

        Task<Transaction?> GetByIdAndUserIdAsync(string userId, int id);

        Task<List<Transaction>> GetDeletedByUserIdAsync(string userId);
        Task<Transaction?> GetDeletedByIdAndUserIdAsync(string userId, int id);

        void Add(Transaction transaction);

        void Update(Transaction transaction);

        Task<int> SaveChangesAsync();
    }
}
