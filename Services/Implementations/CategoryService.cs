using Microsoft.EntityFrameworkCore;
using MonthlyExpenseTracker.Common;
using MonthlyExpenseTracker.Data;
using MonthlyExpenseTracker.Models;
using MonthlyExpenseTracker.Repositories.Interfaces;
using MonthlyExpenseTracker.Services.Interfaces;
using MonthlyExpenseTracker.ViewModels.Categories;
using System.Xml.Linq;

namespace MonthlyExpenseTracker.Services.Implementations;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _categoryRepository;
    
    public CategoryService(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }
    public async Task<ServiceResult> CreateAsync(CategoryCreateViewModel model, string userId)
    {
        var name = model.Name.Trim();
        var isExistCategory = await _categoryRepository.ExistsByNameAsync(userId, name);

        if (isExistCategory)
        {
            return ServiceResult.Failure(ErrorMessages.CategoryNameExists, name);
        }

        var category = new Category
        {
            Name = name,
            ApplicationUserId = userId,
            IsActive = true,
            UpdatedAt = DateTime.UtcNow
        };

        _categoryRepository.Add(category);
        await _categoryRepository.SaveChangesAsync();

        return ServiceResult.Success();
    }
    public async Task<List<CategoryListItemViewModel>> GetCategoriesAsync(string userId)
    {
        var categories = await _categoryRepository.GetByUserIdAsync(userId);

        return [.. categories.Select(x => new CategoryListItemViewModel
        {
            Id = x.Id,
            Name = x.Name,
            IsActive = x.IsActive
        })];
    }

    public async Task<CategoryEditViewModel?> GetEditModelAsync(int categoryId, string userId)
    {
        var category = await _categoryRepository.GetByIdAndUserIdAsync(categoryId, userId);

        if (category == null)
        {
            return null;
        }

        return new CategoryEditViewModel
        {
            Id = category.Id,
            Name = category.Name,
            IsActive = category.IsActive
        };
    }

    public async Task<ServiceResult> UpdateAsync(CategoryEditViewModel model, string userId)
    {
        var category = await _categoryRepository.GetByIdAndUserIdAsync(model.Id, userId);
        if (category == null)
        {
            return ServiceResult.Failure(ErrorMessages.CategoryNotFound);
        }

        var categoryId = model.Id;
        var categoryName = model.Name.Trim();
        var isExistCategory = await _categoryRepository.ExistsByNameExceptIdAsync(userId, categoryId, categoryName);

        if (isExistCategory)
        {
            return ServiceResult.Failure(ErrorMessages.CategoryNameExists);
        }

        category.Name = categoryName;
        category.IsActive = model.IsActive;
        category.UpdatedAt = DateTime.UtcNow;

        _categoryRepository.Update(category);
        await _categoryRepository.SaveChangesAsync();

        return ServiceResult.Success();
    }

    public async Task<ServiceResult> SetStatusAsync(CategoryEditViewModel model, string userId, bool isActive)
    {
        var category = await _categoryRepository.GetByIdAndUserIdAsync(model.Id, userId);
        if (category == null)
        {
            return ServiceResult.Failure(ErrorMessages.CategoryNotFound);
        }

        category.IsActive = isActive;
        category.UpdatedAt = DateTime.UtcNow;

        _categoryRepository.Update(category);

        await _categoryRepository.SaveChangesAsync();

        return ServiceResult.Success();
    }
}