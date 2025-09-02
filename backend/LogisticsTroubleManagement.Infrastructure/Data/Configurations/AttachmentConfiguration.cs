using LogisticsTroubleManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LogisticsTroubleManagement.Infrastructure.Data.Configurations;

public class AttachmentConfiguration : IEntityTypeConfiguration<Attachment>
{
	public void Configure(EntityTypeBuilder<Attachment> builder)
	{
		builder.ToTable("Attachments");
		builder.HasKey(a => a.Id);
		builder.Property(a => a.Id).ValueGeneratedOnAdd();

		builder.Property(a => a.FileName)
			.IsRequired()
			.HasMaxLength(255);
		builder.Property(a => a.FilePath)
			.IsRequired()
			.HasMaxLength(500);
		builder.Property(a => a.FileSize)
			.IsRequired();
		builder.Property(a => a.ContentType)
			.IsRequired()
			.HasMaxLength(100);
		builder.Property(a => a.UploadedAt)
			.HasDefaultValueSql("GETUTCDATE()")
			.IsRequired();
		builder.Property(a => a.CreatedAt).HasDefaultValueSql("GETUTCDATE()").IsRequired();
		builder.Property(a => a.UpdatedAt).IsRequired(false);

		builder.HasOne(a => a.Incident)
			.WithMany(i => i.Attachments)
			.HasForeignKey(a => a.IncidentId)
			.OnDelete(DeleteBehavior.Cascade);

		builder.HasOne(a => a.UploadedBy)
			.WithMany(u => u.UploadedAttachments)
			.HasForeignKey(a => a.UploadedById)
			.OnDelete(DeleteBehavior.NoAction);
	}
}
