using LogisticsTroubleManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LogisticsTroubleManagement.Infrastructure.Data.Configurations;

public class EffectivenessConfiguration : IEntityTypeConfiguration<Effectiveness>
{
	public void Configure(EntityTypeBuilder<Effectiveness> builder)
	{
		builder.ToTable("Effectiveness");
		builder.HasKey(e => e.Id);
		builder.Property(e => e.Id).ValueGeneratedOnAdd();

		builder.Property(e => e.EffectivenessType)
			.IsRequired()
			.HasMaxLength(50);
		builder.Property(e => e.Value)
			.IsRequired()
			.HasColumnType("decimal(10,2)");
		builder.Property(e => e.Unit)
			.IsRequired(false)
			.HasMaxLength(20);
		builder.Property(e => e.Description)
			.IsRequired(false)
			.HasMaxLength(500);
		builder.Property(e => e.MeasuredAt)
			.HasDefaultValueSql("GETUTCDATE()")
			.IsRequired();
		builder.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()").IsRequired();
		builder.Property(e => e.UpdatedAt).HasDefaultValueSql("GETUTCDATE()").IsRequired();

		builder.HasOne(e => e.Incident)
			.WithMany(i => i.Effectiveness)
			.HasForeignKey(e => e.IncidentId)
			.OnDelete(DeleteBehavior.Cascade);

		builder.HasOne(e => e.MeasuredBy)
			.WithMany(u => u.MeasuredEffectiveness)
			.HasForeignKey(e => e.MeasuredById)
			.OnDelete(DeleteBehavior.NoAction);
	}
}
