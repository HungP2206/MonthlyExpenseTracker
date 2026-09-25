using System.ComponentModel.DataAnnotations;

namespace MonthlyExpenseTracker.ViewModels.Transaction
{
    public class TransactionEditViewModel
    {
        public Guid Id { get; set; }

        public decimal Amount { get; set; }

        public DateTime TransactionDate { get; set; }

        public string? Note { get; set; }

        public Guid CategoryId { get; set; }

        public bool IsDelete { get; set; }

        public string EditToken { get; set; }

        public List<CategoryOptionViewModel> Categories { get; set; } = new();
    }
}
