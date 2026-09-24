using MonthlyExpenseTracker.Models;

namespace MonthlyExpenseTracker.ViewModels.Transaction
{
    public class TransactionListItemViewModel
    {
        public Guid Id { get; set; }

        public decimal Amount { get; set; }

        public DateTime TransactionDate { get; set; }

        public string? Note { get; set; }

        public string CategoryName { get; set; } = null!;


    }
}
