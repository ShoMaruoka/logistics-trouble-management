using LogisticsTroubleManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LogisticsTroubleManagement.Infrastructure.Data.Configurations;

public class IncidentConfiguration : IEntityTypeConfiguration<Incident>
{
	public void Configure(EntityTypeBuilder<Incident> builder)
	{
		builder.ToTable("Incidents");
		builder.HasKey(i => i.Id);
		builder.Property(i => i.Id).ValueGeneratedOnAdd();

		builder.Property(i => i.Title)
			.IsRequired()
			.HasMaxLength(200);
		builder.Property(i => i.Description)
			.IsRequired();
		builder.Property(i => i.Status)
			.IsRequired();
		builder.Property(i => i.Priority)
			.IsRequired();
		builder.Property(i => i.Category)
			.IsRequired()
			.HasMaxLength(50);

		builder.Property(i => i.ReportedDate)
			.HasDefaultValueSql("GETUTCDATE()")
			.IsRequired();
		builder.Property(i => i.ResolvedDate)
			.IsRequired(false);
		builder.Property(i => i.Resolution)
			.IsRequired(false);

		builder.Property(i => i.CreatedAt).HasDefaultValueSql("GETUTCDATE()").IsRequired();
		builder.Property(i => i.UpdatedAt).HasDefaultValueSql("GETUTCDATE()").IsRequired();

		builder.HasOne(i => i.ReportedBy)
			.WithMany(u => u.ReportedIncidents)
			.HasForeignKey(i => i.ReportedById)
			.OnDelete(DeleteBehavior.NoAction);

		builder.HasOne(i => i.AssignedTo)
			.WithMany(u => u.AssignedIncidents)
			.HasForeignKey(i => i.AssignedToId)
			.OnDelete(DeleteBehavior.NoAction);
	}
}
