using Microsoft.AspNetCore.Identity;

namespace MonthlyExpenseTracker.Models
{
    public class ApplicationUser : IdentityUser
    {
        public ICollection<Category> Categories { get; set; } = new List<Category>();
        public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    }
}

