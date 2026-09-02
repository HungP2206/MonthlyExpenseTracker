using MonthlyExpenseTracker.Models;

namespace MonthlyExpenseTracker.Repositories.Interfaces
{
    public interface ICategoryRepository
    {
        Task<List<Category>> GetByUserIdAsync(string userId);

        Task<Category?> GetByIdAndUserIdAsync(int id, string userId);

        Task<bool> ExistsByNameAsync(string userId, string name);

        Task<bool> ExistsByNameExceptIdAsync(string userId, int Id, string name);

        Task<bool> IsUsedAsync(string userId, int Id);

        void Add(Category category);

        void Update(Category category);

        void Delete(Category category);

        Task<int> SaveChangesAsync();
    }
}
