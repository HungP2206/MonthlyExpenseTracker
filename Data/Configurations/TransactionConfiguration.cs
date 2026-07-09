using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MonthlyExpenseTracker.Models;

namespace MonthlyExpenseTracker.Data.Configurations;

public class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
{
    public void Configure(EntityTypeBuilder<Transaction> builder)
    {
        // Tên bảng trong PostgreSQL
        builder.ToTable("Transactions");

        // Primary Key
        builder.HasKey(x => x.Id);

        // PostgreSQL tự tăng Id
        builder.Property(x => x.Id)
            .ValueGeneratedOnAdd();

        // Số tiền là bắt buộc
        // precision 18,2:
        // tổng cộng tối đa 18 chữ số, trong đó có 2 chữ số thập phân
        builder.Property(x => x.Amount)
            .IsRequired()
            .HasPrecision(18, 2);

        // Ngày phát sinh giao dịch là bắt buộc
        builder.Property(x => x.TransactionDate)
            .IsRequired();

        // Note không bắt buộc vì entity dùng string?
        builder.Property(x => x.Note)
            .HasMaxLength(500);

        // Foreign key đến Category
        builder.Property(x => x.CategoryId)
            .IsRequired();

        // IdentityUser.Id có kiểu string
        builder.Property(x => x.ApplicationUserId)
            .IsRequired()
            .HasMaxLength(450);

        // Chưa từng cập nhật thì có thể null
        builder.Property(x => x.UpdatedAt)
            .IsRequired(false);

        // Transaction mới mặc định chưa bị xóa
        builder.Property(x => x.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        // Transaction thuộc về một Category
        // Một Category có nhiều Transaction
        builder.HasOne(x => x.Category)
            .WithMany(x => x.Transactions)
            .HasForeignKey(x => x.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        // Transaction thuộc về một ApplicationUser
        // Một ApplicationUser có nhiều Transaction
        builder.HasOne(x => x.ApplicationUser)
            .WithMany(x => x.Transactions)
            .HasForeignKey(x => x.ApplicationUserId)
            .OnDelete(DeleteBehavior.Restrict);

        // Tự động loại bỏ Transaction đã soft delete
        // khỏi các truy vấn thông thường
        builder.HasQueryFilter(x => !x.IsDeleted);

        // Hỗ trợ truy vấn Transaction theo user
        builder.HasIndex(x => x.ApplicationUserId);

        // Hỗ trợ truy vấn Transaction theo Category
        builder.HasIndex(x => x.CategoryId);

        // Hỗ trợ dashboard:
        // lấy giao dịch của user theo khoảng ngày
        builder.HasIndex(x => new
        {
            x.ApplicationUserId,
            x.TransactionDate
        });
    }
}