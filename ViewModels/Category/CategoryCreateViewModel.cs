using System.ComponentModel.DataAnnotations;

namespace MonthlyExpenseTracker.ViewModels.Category
{
    public class CategoryCreateViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập tên danh mục.")]
        [StringLength(100, ErrorMessage = "Tên danh mục không được vượt quá 100 ký tự.")]
        public string Name { get; set; } = string.Empty;
    }
}
