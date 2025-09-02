using LogisticsTroubleManagement.Domain.Entities;
using LogisticsTroubleManagement.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace LogisticsTroubleManagement.Infrastructure.Data.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
	public void Configure(EntityTypeBuilder<User> builder)
	{
		builder.ToTable("Users");

		builder.HasKey(u => u.Id);
		builder.Property(u => u.Id).ValueGeneratedOnAdd();

		builder.Property(u => u.Username)
			.IsRequired()
			.HasMaxLength(50);
		builder.HasIndex(u => u.Username).IsUnique();

		builder.Property(u => u.Email)
			.IsRequired()
			.HasConversion(
				v => v.Value,
				v => Email.Create(v))
			.HasMaxLength(100);
		builder.HasIndex("Email").IsUnique();

		builder.Property(u => u.FirstName)
			.IsRequired()
			.HasMaxLength(50);
		builder.Property(u => u.LastName)
			.IsRequired()
			.HasMaxLength(50);

		builder.Property(u => u.Role)
			.IsRequired();

		builder.Property(u => u.IsActive)
			.IsRequired()
			.HasDefaultValue(true);

		var phoneNumberConverter = new ValueConverter<PhoneNumber?, string?>(
			v => v == null ? null : v.Value,
			v => v == null ? null : PhoneNumber.Create(v)
		);

		builder.Property(u => u.PhoneNumber)
			.HasConversion(phoneNumberConverter)
			.HasMaxLength(20)
			.IsRequired(false);

		builder.Property(u => u.CreatedAt)
			.HasDefaultValueSql("GETUTCDATE()")
			.IsRequired();
		builder.Property(u => u.UpdatedAt)
			.IsRequired(false);

		builder.HasMany(u => u.ReportedIncidents)
			.WithOne(i => i.ReportedBy)
			.HasForeignKey(i => i.ReportedById)
			.OnDelete(DeleteBehavior.NoAction);

		builder.HasMany(u => u.AssignedIncidents)
			.WithOne(i => i.AssignedTo)
			.HasForeignKey(i => i.AssignedToId)
			.OnDelete(DeleteBehavior.NoAction);

		builder.HasMany(u => u.UploadedAttachments)
			.WithOne(a => a.UploadedBy)
			.HasForeignKey(a => a.UploadedById)
			.OnDelete(DeleteBehavior.NoAction);

		builder.HasMany(u => u.AuditLogs)
			.WithOne(a => a.User)
			.HasForeignKey(a => a.UserId)
			.OnDelete(DeleteBehavior.NoAction);

		builder.HasMany(u => u.MeasuredEffectiveness)
			.WithOne(e => e.MeasuredBy)
			.HasForeignKey(e => e.MeasuredById)
			.OnDelete(DeleteBehavior.NoAction);
	}
}
