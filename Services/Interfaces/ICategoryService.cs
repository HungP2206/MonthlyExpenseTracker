using MonthlyExpenseTracker.ViewModels.Categories;
using MonthlyExpenseTracker.Common;
namespace MonthlyExpenseTracker.Services.Interfaces;

public interface ICategoryService
{
    Task<ServiceResult> CreateAsync(CategoryCreateViewModel model, string userId);

    Task<List<CategoryListItemViewModel>> GetCategoriesAsync(string userId);

    Task<CategoryEditViewModel?> GetEditModelAsync(int categoryId, string userId);

    Task<ServiceResult> UpdateAsync(CategoryEditViewModel model, string userId);

    Task<ServiceResult> SetStatusAsync(CategoryEditViewModel model, string userId, bool isActive);

}