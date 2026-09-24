using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MonthlyExpenseTracker.Services.Interfaces;
using MonthlyExpenseTracker.ViewModels.Transaction;
using System.Security.Claims;

namespace MonthlyExpenseTracker.Controllers
{
    [Authorize]
    public class TransactionController : Controller
    {
        private readonly ITransactionService _transactionService;

        public TransactionController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
            {
                return Unauthorized();
            }

            var transactions = await _transactionService.GetByUserIdAsync(userId);

            return View(transactions);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
            {
                return Unauthorized();
            }

            var model = await _transactionService.GetTransactionCreateViewModelAsync(userId);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TransactionCreateViewModel model)
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

            var result = await _transactionService.CreateAsync(model, userId);

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
        public async Task<IActionResult> Edit(Guid transactionId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
            {
                return Unauthorized();
            }

            var model = await _transactionService.GetEditModelAsync(transactionId, userId);

            if (model == null)
            {
                return NotFound();
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(TransactionEditViewModel model, int transactionId)

        public async Task<IActionResult> Edit(TransactionEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            //if (transactionId != model.Id)
            //{
            //    return NotFound();
            //}

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
            {
                return Unauthorized();
            }

            var result = await _transactionService.UpdateAsync(model, userId);

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
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid transactionId)
        {

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
            {
                return Unauthorized();
            }

            var result = await _transactionService.DeleteAsync(transactionId, userId);

            if (!result.Succeeded)
            {
                return NotFound();
                //TempData["ErrorMessage"] = result.ErrorMessage;
                //return RedirectToAction(nameof(Index));
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Restore(Guid transactionId)
        {

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
            {
                return Unauthorized();
            }

            var result = await _transactionService.RestoreAsync(transactionId, userId);

            if (!result.Succeeded)
            {
                return NotFound();
            }
            return RedirectToAction(nameof(Index));
        }


    }
}
