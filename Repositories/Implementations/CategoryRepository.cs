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
            .Where(x => x.ApplicationUserId == userId)
            .OrderBy(x => x.Name)
            .ToListAsync();
    }

    public async Task<Category?> GetByIdAsync(int id)
    {
        return await _context.Categories
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<Category?> GetByIdAndUserIdAsync(int id, string userId)
    {
        return await _context.Categories
            .FirstOrDefaultAsync(x =>
                x.Id == id &&
                x.ApplicationUserId == userId);
    }

    public async Task<bool> ExistsByNameAsync(string userId, string name)
    {
        return await _context.Categories
            .AnyAsync(x =>
                x.ApplicationUserId == userId &&
                x.Name == name);
    }

    public async Task<bool> IsUsedAsync(int categoryId)
    {
        return await _context.Transactions
            .AnyAsync(x => x.CategoryId == categoryId);
    }

    public async Task AddAsync(Category category)
    {
        await _context.Categories.AddAsync(category);
    }

    public void Update(Category category)
    {
        _context.Categories.Update(category);
    }

    public void Remove(Category category)
    {
        _context.Categories.Remove(category);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}