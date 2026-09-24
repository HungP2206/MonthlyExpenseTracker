namespace MonthlyExpenseTracker.ViewModels.Category
{
    public class CategoryListItemViewModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public bool IsActive { get; set; }
    }
}
