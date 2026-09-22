using System.ComponentModel.DataAnnotations;

namespace MonthlyExpenseTracker.ViewModels.Transaction
{
    public class TransactionEditViewModel
    {
        public int Id { get; set; }

        public decimal Amount { get; set; }

        public DateTime TransactionDate { get; set; }

        public string? Note { get; set; }

        public int CategoryId { get; set; }

        public bool isDelete { get; set; }

        public List<CategoryOptionViewModel> Categories { get; set; } = new();
    }
}
