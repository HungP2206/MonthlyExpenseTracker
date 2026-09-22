using System.ComponentModel.DataAnnotations;

namespace MonthlyExpenseTracker.ViewModels.Category
{
    public class CategoryEditViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên danh mục.")]
        [StringLength(100, ErrorMessage = "Tên danh mục không được vượt quá 100 ký tự.")]
        public string Name { get; set; } = string.Empty;

        public bool IsActive { get; set; }
    }
}
