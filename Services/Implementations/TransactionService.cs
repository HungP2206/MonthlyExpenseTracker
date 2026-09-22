using MonthlyExpenseTracker.Common;
using MonthlyExpenseTracker.Models;
using MonthlyExpenseTracker.Repositories.Interfaces;
using MonthlyExpenseTracker.Services.Interfaces;
using MonthlyExpenseTracker.ViewModels.Transaction;

namespace MonthlyExpenseTracker.Services.Implementations;

public class TransactionService : ITransactionService
{
    private readonly ITransactionRepository _transactionRepository;
    private readonly ICategoryRepository _categoryRepository;

    public TransactionService(ITransactionRepository transactionRepository, ICategoryRepository categoryRepository)
    {
        _transactionRepository = transactionRepository;
        _categoryRepository = categoryRepository;
    }

    public async Task<TransactionCreateViewModel> GetTransactionCreateViewModelAsync(string userId)
    {
        var categories =  await _categoryRepository.GetActiveByUserIdAsync(userId);

        return new TransactionCreateViewModel
        {
            TransactionDate = DateTime.Today,

            Categories = categories.Select(x => new CategoryOptionViewModel
             {
                 Id = x.Id,
                 Name = x.Name
             }).ToList()
        };
    }

    public async Task<ServiceResult> CreateAsync(TransactionCreateViewModel model, string userId)
    {
        var category = await _categoryRepository.GetByIdAndUserIdAsync(userId, model.CategoryId);

        if (category == null)
        {
            return ServiceResult.Failure(ErrorMessages.CategoryNotFound, nameof(TransactionCreateViewModel.CategoryId));
        }
        if (!category.IsActive)
        {
            return ServiceResult.Failure(ErrorMessages.CategoryDeactivated, nameof(TransactionCreateViewModel.CategoryId));
        }

        var transaction = new Transaction
        {
            Amount = model.Amount,
            TransactionDate = model.TransactionDate,
            Note = model.Note,
            CategoryId = model.CategoryId,
            ApplicationUserId = userId,
            UpdatedAt = DateTime.UtcNow,
            IsDeleted = false
        };

        _transactionRepository.Add(transaction);
        await _transactionRepository.SaveChangesAsync();

        return ServiceResult.Success();
    }
    
    public async Task<List<TransactionListItemViewModel>> GetByUserIdAsync(string userId)
    {
        var transactions = await _transactionRepository.GetByUserIdAsync(userId);

        return [.. transactions.Select(x => new TransactionListItemViewModel
        {
            Id = x.Id,
            Amount = x.Amount,
            TransactionDate = x.TransactionDate,
            CategoryName = x.Category.Name,
            Note = x.Note,
        })];
    }

    public async Task<TransactionEditViewModel?> GetEditModelAsync(int transactionId, string userId)
    {
        var transaction = await _transactionRepository.GetByIdAndUserIdAsync(userId, transactionId);

        if (transaction == null)
        {
            return null;
        }
        var categories = await _categoryRepository.GetByUserIdAsync(userId);

        return new TransactionEditViewModel
        {
            Id = transaction.Id,
            Amount = transaction.Amount,
            TransactionDate = transaction.TransactionDate,
            CategoryId = transaction.CategoryId,
            Note = transaction.Note,
            Categories = categories
            .Where(x => x.IsActive || x.Id == transaction.CategoryId)
            .Select(x => new CategoryOptionViewModel{
                Id = x.Id,
                Name = x.Name
            }).ToList()
        };
    }
    public async Task<ServiceResult> UpdateAsync(TransactionEditViewModel model, string userId)
    {
        var transaction = await _transactionRepository.GetByIdAndUserIdAsync(userId, model.Id);
        if (transaction == null)
        {
            return ServiceResult.Failure(ErrorMessages.TransactionNotFound);
        }
        if (transaction.CategoryId != model.CategoryId) { 
            var category = await _categoryRepository.GetByIdAndUserIdAsync(userId, model.CategoryId);

            if (category == null)
            {
                return ServiceResult.Failure(ErrorMessages.CategoryNotFound, nameof(TransactionEditViewModel.CategoryId));
            }
            if (!category.IsActive)
            {
                return ServiceResult.Failure(ErrorMessages.CategoryDeactivated, nameof(TransactionEditViewModel.CategoryId));
            }
        }

        transaction.Amount = model.Amount;
        transaction.TransactionDate = model.TransactionDate;
        transaction.CategoryId = model.CategoryId;
        transaction.Note = model.Note;
        transaction.UpdatedAt = DateTime.UtcNow;

        _transactionRepository.Update(transaction);
        await _transactionRepository.SaveChangesAsync();

        return ServiceResult.Success();
    }

    public async Task<ServiceResult> DeleteAsync(int transactionId, string userId)
    {
        var transaction = await _transactionRepository.GetByIdAndUserIdAsync(userId, transactionId);
        if (transaction == null)
        {
            return ServiceResult.Failure(ErrorMessages.TransactionNotFound);
        }

        transaction.IsDeleted = true;
        transaction.UpdatedAt = DateTime.UtcNow;

        _transactionRepository.Update(transaction);
        await _transactionRepository.SaveChangesAsync();

        return ServiceResult.Success();
    }

    public async Task<ServiceResult> RestoreAsync(int transactionId, string userId)
    {
        var transaction = await _transactionRepository.GetByIdAndUserIdAsync(userId, transactionId);
        if (transaction == null)
        {
            return ServiceResult.Failure(ErrorMessages.TransactionNotFound);
        }

        transaction.IsDeleted = false;
        transaction.UpdatedAt = DateTime.UtcNow;

        _transactionRepository.Update(transaction);
        await _transactionRepository.SaveChangesAsync();

        return ServiceResult.Success();
    }

}