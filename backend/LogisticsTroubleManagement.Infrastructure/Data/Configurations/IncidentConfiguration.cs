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

		// 新規追加プロパティ
		builder.Property(i => i.ClassificationNotes)
			.IsRequired(false)
			.HasMaxLength(500);
		builder.Property(i => i.ExpectedResolutionDate)
			.IsRequired(false);

		// WorkflowStatusは削除 - 新仕様ではStatusに統合
		
		builder.Property(i => i.DueDate)
			.IsRequired(false)
			.HasComment("対応期限");
		
		// ワークフロー進捗日付
		builder.Property(i => i.ResponseStartDate)
			.IsRequired(false)
			.HasComment("対応開始日");
		
		builder.Property(i => i.CauseAnalysisDate)
			.IsRequired(false)
			.HasComment("原因入力日");
		
		builder.Property(i => i.CompletionDate)
			.IsRequired(false)
			.HasComment("対応完了日");
		
		builder.Property(i => i.PreventionProposalDate)
			.IsRequired(false)
			.HasComment("再発防止策提案日");
		
		builder.Property(i => i.EffectivenessConfirmationDate)
			.IsRequired(false)
			.HasComment("有効性確認日");
		
		// 新作業内容フィールド
		builder.Property(i => i.ResponseContent)
			.IsRequired(false)
			.HasMaxLength(2000)
			.HasComment("対応内容");

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
