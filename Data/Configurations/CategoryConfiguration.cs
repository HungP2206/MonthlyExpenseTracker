using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MonthlyExpenseTracker.Models;

namespace MonthlyExpenseTracker.Data.Configurations;

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable("Categories");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.ApplicationUserId)
            .IsRequired()
            .HasMaxLength(450);

        builder.Property(x => x.IsActive)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(x => x.UpdatedAt)
            .IsRequired(false);

        builder.HasOne(x => x.ApplicationUser)
            .WithMany(x => x.Categories)
            .HasForeignKey(x => x.ApplicationUserId)
            .OnDelete(DeleteBehavior.Restrict);

        //builder.HasQueryFilter(x => !x.IsActive);

        builder.HasIndex(x => x.ApplicationUserId);

        builder.HasIndex(x => new
        {
            x.ApplicationUserId,
            x.Name
        });

        //builder.HasIndex(x => new { x.ApplicationUserId, x.Name })
        //    .IsUnique();
    }
}