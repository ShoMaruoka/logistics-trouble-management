using LogisticsTroubleManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LogisticsTroubleManagement.Infrastructure.Data.Configurations;

public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
{
	public void Configure(EntityTypeBuilder<AuditLog> builder)
	{
		builder.ToTable("AuditLogs");
		builder.HasKey(a => a.Id);
		builder.Property(a => a.Id).ValueGeneratedOnAdd();

		builder.Property(a => a.Action)
			.IsRequired()
			.HasMaxLength(50);
		builder.Property(a => a.TableName)
			.IsRequired()
			.HasMaxLength(50);
		builder.Property(a => a.RecordId).IsRequired(false);
		builder.Property(a => a.OldValues).IsRequired(false);
		builder.Property(a => a.NewValues).IsRequired(false);
		builder.Property(a => a.IpAddress).IsRequired(false).HasMaxLength(45);
		builder.Property(a => a.UserAgent).IsRequired(false).HasMaxLength(500);
		builder.Property(a => a.CreatedAt).HasDefaultValueSql("GETUTCDATE()").IsRequired();
		builder.Property(a => a.UpdatedAt).IsRequired(false);

		builder.HasOne(a => a.User)
			.WithMany(u => u.AuditLogs)
			.HasForeignKey(a => a.UserId)
			.OnDelete(DeleteBehavior.NoAction);
	}
}
