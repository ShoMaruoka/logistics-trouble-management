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
			.HasMaxLength(100);
		builder.Property(e => e.BeforeValue)
			.IsRequired()
			.HasColumnType("decimal(10,2)");
		builder.Property(e => e.AfterValue)
			.IsRequired()
			.HasColumnType("decimal(10,2)");
		builder.Property(e => e.ImprovementRate)
			.IsRequired()
			.HasColumnType("decimal(5,2)");
		builder.Property(e => e.Description)
			.IsRequired()
			.HasMaxLength(1000);
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
