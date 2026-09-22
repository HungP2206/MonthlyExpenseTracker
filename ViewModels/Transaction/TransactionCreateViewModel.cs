using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace MonthlyExpenseTracker.ViewModels.Transaction
{
    public class TransactionCreateViewModel
    {
        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Amount { get; set; }

        [Required]
        public DateTime TransactionDate { get; set; }

        [StringLength(500)]
        public string? Note { get; set; }

        [Required]
        public int CategoryId { get; set; }

        public List<CategoryOptionViewModel> Categories { get; set; } = new();
    }
}
