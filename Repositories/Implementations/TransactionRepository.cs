using Microsoft.EntityFrameworkCore;
using MonthlyExpenseTracker.Data;
using MonthlyExpenseTracker.Models;
using MonthlyExpenseTracker.Repositories.Interfaces;

namespace MonthlyExpenseTracker.Repositories.Implementations;

public class TransactionRepository : ITransactionRepository
{
    private readonly ApplicationDbContext _context;

    public TransactionRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Transaction>> GetByUserIdAsync(string userId)
    {
        return await _context.Transactions
            .AsNoTracking()
            .Include(x => x.Category)
            .Where(x => x.ApplicationUserId == userId && !x.IsDeleted)
            .OrderByDescending(x => x.TransactionDate)
            .ToListAsync();
    }

    public async Task<Transaction?> GetByIdAndUserIdAsync(string userId, int transactionId)
    {
        return await _context.Transactions
            .FirstOrDefaultAsync(x =>
            x.Id == transactionId && 
            x.ApplicationUserId == userId &&
            !x.IsDeleted);
    }

    public async Task<List<Transaction>> GetDeletedByUserIdAsync(string userId)
    {
        return await _context.Transactions
           .AsNoTracking()
           .Where(x => x.ApplicationUserId == userId && x.IsDeleted)
           .OrderByDescending(x => x.TransactionDate)
           .ToListAsync();
    }
    public async Task<Transaction?> GetDeletedByIdAndUserIdAsync(string userId, int transactionId)
    {
        return await _context.Transactions
            .FirstOrDefaultAsync(x =>
                x.Id == transactionId &&
                x.ApplicationUserId == userId &&
                x.IsDeleted);
    }
    public void Add(Transaction transaction)
    {
        _context.Transactions.Add(transaction);
    }

    public void Update(Transaction transaction)
    {
        _context.Transactions.Update(transaction);
    }

    public void Delete(Transaction transaction)
    {
        _context.Transactions.Remove(transaction);
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

}