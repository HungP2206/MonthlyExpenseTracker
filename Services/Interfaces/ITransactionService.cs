using MonthlyExpenseTracker.ViewModels.Transaction;
using MonthlyExpenseTracker.Common;
namespace MonthlyExpenseTracker.Services.Interfaces;

public interface ITransactionService
{
    Task<List<TransactionListItemViewModel>> GetByUserIdAsync(string userId);

    Task<ServiceResult> CreateAsync(TransactionCreateViewModel model, string userId);

    Task<TransactionCreateViewModel> GetTransactionCreateViewModelAsync(string userId);

    Task<TransactionEditViewModel> GetEditModelAsync(int id, string userId);

    Task<ServiceResult> UpdateAsync(TransactionEditViewModel model, string userId);

    Task<ServiceResult> DeleteAsync(int transactionId, string userId);

    Task<ServiceResult> RestoreAsync(int transactionId, string userId);


}