using Microsoft.EntityFrameworkCore;
using MonthlyExpenseTracker.Data;
using MonthlyExpenseTracker.Models;
using MonthlyExpenseTracker.Repositories.Interfaces;

namespace MonthlyExpenseTracker.Repositories.Implementations;

public class CategoryRepository : ICategoryRepository
{
    private readonly ApplicationDbContext _context;

    public CategoryRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Category>> GetByUserIdAsync(string userId)
    {
        return await _context.Categories
            .AsNoTracking()
            .Where(x => x.ApplicationUserId == userId)
            .OrderBy(x => x.Name)
            .ToListAsync();
    }

    public async Task<Category?> GetByIdAndUserIdAsync(int categoryId, string userId)
    {
        return await _context.Categories
            .FirstOrDefaultAsync(x =>
            x.Id == categoryId &&
            x.ApplicationUserId == userId);
    }
    public async Task<bool> ExistsByNameAsync(string userId, string CategoryName)
    {
        return await _context.Categories
            .AnyAsync(x => 
            x.ApplicationUserId == userId &&
            x.Name == CategoryName);
    }

    public async Task<bool> ExistsByNameExceptIdAsync(string userId, int excludedCategoryId, string name)
    {
        return await _context.Categories
            .AnyAsync(x =>
                x.ApplicationUserId == userId &&
                x.Name == name &&
                x.Id != excludedCategoryId);
    }

    public async Task<bool> IsUsedAsync(string userId, int CategoryId)
    {
        return await _context.Categories
            .AnyAsync(x =>
                x.ApplicationUserId == userId &&
                x.Id != CategoryId);
    }

    public void Add(Category category)
    {
         _context.Categories.Add(category);
    }

    public void Update(Category category)
    {
        _context.Categories.Update(category);
    }

    public void Delete(Category category)
    {
        _context.Categories.Remove(category);
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

}