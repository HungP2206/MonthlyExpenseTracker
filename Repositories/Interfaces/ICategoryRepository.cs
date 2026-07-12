using MonthlyExpenseTracker.Models;

namespace MonthlyExpenseTracker.Repositories.Interfaces
{
    public interface ICategoryRepository
    {
        Task<List<Category>> GetByUserIdAsync(string userId);

        Task<Category?> GetByIdAsync(int id);

        Task<Category?> GetByIdAndUserIdAsync(int id, string userId);

        Task<bool> ExistsByNameAsync(string userId, string name);

        Task<bool> IsUsedAsync(int categoryId);

        Task AddAsync(Category category);

        void Update(Category category);

        void Remove(Category category);

        Task SaveChangesAsync();
    }
}
