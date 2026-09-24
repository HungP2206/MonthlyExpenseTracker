using System.ComponentModel.DataAnnotations;

namespace MonthlyExpenseTracker.Models
{
    public class Category
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string ApplicationUserId { get; set; } = null!;
        public bool IsActive { get; set; } = true;

        public DateTime? UpdatedAt { get; set; }

        public ApplicationUser ApplicationUser { get; set; } = null!;

        public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    }
}
