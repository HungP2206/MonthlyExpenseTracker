using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using MonthlyExpenseTracker.Services.Interfaces;
using MonthlyExpenseTracker.ViewModels.Category;
using System.Globalization;
using System.Security.Claims;

namespace MonthlyExpenseTracker.Controllers
{
    [Authorize]
    public class CategoryController : Controller
    {
        private readonly ICategoryService _categoryService;
        private readonly IDataProtectionProvider _protectionProvider;

        public CategoryController(ICategoryService categoryService, IDataProtectionProvider dataProtectionProvider)
        {
            _categoryService = categoryService;
            _protectionProvider = dataProtectionProvider;
        }

        private ITimeLimitedDataProtector GetEditProtector(string userId)
        {
            var categoryProtector = _protectionProvider.CreateProtector("Category.Edit.v1");

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

            if (string.IsNullOrWhiteSpace(model.EditToken))
            {
                return BadRequest();
            }

            Guid originalCategoryId;

            try
            {
                var categoryIdText = GetEditProtector(userId).Unprotect(
                    model.EditToken, out _);

                if (!Guid.TryParse(categoryIdText, out originalCategoryId) || originalCategoryId == Guid.Empty)
                {
                    return BadRequest();
                }
            }
            catch
            {
                return BadRequest();
            }

            if (model.Id != originalCategoryId)
            {
                return BadRequest(
                    "Danh mục gửi lên không khớp với danh mục đã mở. " +
                    "Vui lòng mở lại trang Edit.");
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
