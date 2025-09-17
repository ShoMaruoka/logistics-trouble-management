using LogisticsTroubleManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LogisticsTroubleManagement.Infrastructure.Data.Configurations;

public class WarehouseConfiguration : IEntityTypeConfiguration<Warehouse>
{
    public void Configure(EntityTypeBuilder<Warehouse> builder)
    {
        builder.ToTable("Warehouses");

        builder.HasKey(w => w.Id);
        builder.Property(w => w.Id).ValueGeneratedOnAdd();

        builder.Property(w => w.Name)
            .IsRequired()
            .HasMaxLength(100);
        builder.HasIndex(w => w.Name).IsUnique();

        builder.Property(w => w.Description)
            .HasMaxLength(500);

        builder.Property(w => w.Location)
            .HasMaxLength(200);

        builder.Property(w => w.ContactInfo)
            .HasMaxLength(200);

        builder.Property(w => w.SortOrder)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(w => w.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(w => w.CreatedAt)
            .HasDefaultValueSql("GETUTCDATE()")
            .IsRequired();

        builder.Property(w => w.UpdatedAt)
            .HasDefaultValueSql("GETUTCDATE()")
            .IsRequired();

        // Navigation properties
        builder.HasMany(w => w.Incidents)
            .WithOne(i => i.Warehouse)
            .HasForeignKey(i => i.WarehouseId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(w => w.Users)
            .WithOne(u => u.Warehouse)
            .HasForeignKey(u => u.WarehouseId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
