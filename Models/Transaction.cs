namespace MonthlyExpenseTracker.Models
{
    public class Transaction
    {
        public int Id { get; set; }

        public decimal Amount { get; set; }

        public DateTime TransactionDate { get; set; }

        public string? Note { get; set; }

        public int CategoryId { get; set; }

        public string ApplicationUserId { get; set; } = null!;

        public DateTime? UpdatedAt { get; set; }

        public bool IsDeleted { get; set; }

        public Category Category { get; set; } = null!;

        public ApplicationUser ApplicationUser { get; set; } = null!;
    }
}
