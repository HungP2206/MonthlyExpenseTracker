namespace MonthlyExpenseTracker.ViewModels.Categories
{
    public class CategoryListItemViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public bool IsActive { get; set; }
    }
}
