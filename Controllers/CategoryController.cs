using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MonthlyExpenseTracker.Services.Interfaces;
using MonthlyExpenseTracker.ViewModels.Categories;
using System.Net.WebSockets;
using System.Security.Claims;

namespace MonthlyExpenseTracker.Controllers
{
    [Authorize]
    public class CategoryController : Controller
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

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
        public async Task<IActionResult> Edit(int categoryId)
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

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(CategoryEditViewModel model)
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
