using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using MonthlyExpenseTracker.Services.Interfaces;
using MonthlyExpenseTracker.ViewModels.Transaction;
using System.Security.Claims;
using System.Security.Cryptography;

namespace MonthlyExpenseTracker.Controllers
{
    [Authorize]
    public class TransactionController : Controller
    {
        private readonly ITransactionService _transactionService;
        private readonly IDataProtectionProvider _dataProtectionProvider;

        public TransactionController(ITransactionService transactionService, IDataProtectionProvider dataProtectionProvider)
        {
            _transactionService = transactionService;
            _dataProtectionProvider = dataProtectionProvider;
        }

        private ITimeLimitedDataProtector GetEditProtector(string userId)
        {
            var categoryProtector = _dataProtectionProvider.CreateProtector("Transaction.Edit.v1");

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

            model.EditToken = GetEditProtector(userId).Protect(
                model.Id.ToString("D"),
                TimeSpan.FromDays(30));

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Edit(TransactionEditViewModel model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
            {
                return Unauthorized();
            }

            const string invalidTokenMessage = "Phiên chỉnh sửa không hợp lệ hoặc đã hết hạn. Vui lòng mở lại trang sửa giao dịch từ danh sách.";

            if (string.IsNullOrWhiteSpace(model.EditToken) ||
                model.EditToken.Length % 4 == 1 ||
                model.EditToken.Any(c => !char.IsAsciiLetterOrDigit(c) && c != '-' && c != '_'))
            {
                TempData["ErrorMessage"] = invalidTokenMessage;
                return RedirectToAction(nameof(Index));
            }

            Guid originalTransactionId;

            try
            {
                var transactionIdText = GetEditProtector(userId).Unprotect(
                    model.EditToken, out _);

                if (!Guid.TryParse(transactionIdText, out originalTransactionId) || originalTransactionId == Guid.Empty)
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

            if (model.Id != originalTransactionId)
            {
                TempData["ErrorMessage"] = "Mã giao dịch gửi lên không khớp với giao dịch đã mở. Vui lòng mở lại trang Edit.";
                return RedirectToAction(nameof(Index));
            }

            if (!ModelState.IsValid)
            {
                return View(model);
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
                TempData["ErrorMessage"] = result.ErrorMessage;
                return RedirectToAction(nameof(Index));
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
