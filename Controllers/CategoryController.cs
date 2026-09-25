using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using MonthlyExpenseTracker.Services.Interfaces;
using MonthlyExpenseTracker.ViewModels.Category;
using System.Security.Claims;
using System.Security.Cryptography;

namespace MonthlyExpenseTracker.Controllers
{
    [Authorize]
    public class CategoryController : Controller
    {
        private readonly ICategoryService _categoryService;
        private readonly IDataProtectionProvider _dataProtectionProvider;

        public CategoryController(ICategoryService categoryService, IDataProtectionProvider dataProtectionProvider)
        {
            _categoryService = categoryService;
            _dataProtectionProvider = dataProtectionProvider;
        }

        private ITimeLimitedDataProtector GetEditProtector(string userId)
        {
            var categoryProtector = _dataProtectionProvider.CreateProtector("Category.Edit.v1");

            var userProtector = categoryProtector.CreateProtector(userId);

            var timeLimitedProtector = userProtector.ToTimeLimitedDataProtector();

            return timeLimitedProtector;
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
            {
                return Unauthorized();
            }
            var categories = await _categoryService.GetCategoriesAsync(userId);

            return View(categories);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new CategoryCreateViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoryCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
            {
                return Unauthorized();
            }

            var result = await _categoryService.CreateAsync(model, userId);

            if (!result.Succeeded)
            {
                ModelState.AddModelError(
                    result.FieldName ?? string.Empty,
                    result.ErrorMessage ?? "Error");
                return View(model);
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid categoryId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
            {
                return Unauthorized();
            }

            var model = await _categoryService.GetEditModelAsync(categoryId, userId);

            if (model == null)
            {
                return NotFound();
            }

            model.EditToken = GetEditProtector(userId).Protect(
                model.Id.ToString("D"),
                TimeSpan.FromDays(30));

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CategoryEditViewModel model)
        {

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
            {
                return Unauthorized();
            }

            const string invalidTokenMessage = "Phiên chỉnh sửa không hợp lệ hoặc đã hết hạn. Vui lòng mở lại trang sửa danh mục từ danh sách.";

            if (string.IsNullOrWhiteSpace(model.EditToken) ||
                model.EditToken.Length % 4 == 1 ||
                model.EditToken.Any(c => !char.IsAsciiLetterOrDigit(c) && c != '-' && c != '_'))
            {
                TempData["ErrorMessage"] = invalidTokenMessage;
                return RedirectToAction(nameof(Index));
            }

            Guid originalCategoryId;

            try
            {
                var categoryIdText = GetEditProtector(userId).Unprotect(
                    model.EditToken, out _);

                if (!Guid.TryParse(categoryIdText, out originalCategoryId) || originalCategoryId == Guid.Empty)
                {
                    TempData["ErrorMessage"] = invalidTokenMessage;
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (CryptographicException)
            {
                TempData["ErrorMessage"] = invalidTokenMessage;
                return RedirectToAction(nameof(Index));
            }

            if (model.Id != originalCategoryId)
            {
                TempData["ErrorMessage"] = "Danh mục gửi lên không khớp với danh mục đã mở. Vui lòng mở lại trang Edit.";
                return RedirectToAction(nameof(Index));
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            model.Id = originalCategoryId;

            var result = await _categoryService.UpdateAsync(model, userId);

            if (!result.Succeeded)
            {
                ModelState.AddModelError(
                    result.FieldName ?? string.Empty,
                    result.ErrorMessage ?? "Error");
                return View(model);
            }
            return RedirectToAction(nameof(Index));

        }
        [HttpPost]
        public async Task<IActionResult> SetDeactivateStatus(CategoryEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
            {
                return Unauthorized();
            }

            var result = await _categoryService.SetStatusAsync(model, userId, false);

            if (!result.Succeeded)
            {
                ModelState.AddModelError(
                    result.FieldName ?? string.Empty,
                    result.ErrorMessage ?? "Error");
                return View(model);
            }
            return RedirectToAction(nameof(Index));

        }
        [HttpPost]
        public async Task<IActionResult> SetActivateStatus(CategoryEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
            {
                return Unauthorized();
            }

            var result = await _categoryService.SetStatusAsync(model, userId, true);

            if (!result.Succeeded)
            {
                ModelState.AddModelError(
                    result.FieldName ?? string.Empty,
                    result.ErrorMessage ?? "Error");
                return View(model);
            }
            return RedirectToAction(nameof(Index));

        }
    }
}
