using LogisticsTroubleManagement.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace LogisticsTroubleManagement.Core.DTOs;

/// <summary>
/// インシデント分類用DTO
/// </summary>
public class ClassifyIncidentDto
{
    [Required(ErrorMessage = "インシデントIDは必須です")]
    public int IncidentId { get; set; }

    [Required(ErrorMessage = "トラブル種類は必須です")]
    public int TroubleTypeId { get; set; }

    [Required(ErrorMessage = "損傷種類は必須です")]
    public int DamageTypeId { get; set; }

    [Required(ErrorMessage = "出荷元倉庫は必須です")]
    public int WarehouseId { get; set; }

    [Required(ErrorMessage = "運送会社は必須です")]
    public int ShippingCompanyId { get; set; }

    [Required(ErrorMessage = "出荷総数は必須です")]
    [Range(0, int.MaxValue, ErrorMessage = "出荷総数は0以上の値を入力してください")]
    public int TotalShipments { get; set; }

    [Required(ErrorMessage = "不良品数は必須です")]
    [Range(0, int.MaxValue, ErrorMessage = "不良品数は0以上の値を入力してください")]
    public int DefectiveItems { get; set; }

    [Required(ErrorMessage = "優先度は必須です")]
    public Priority Priority { get; set; }

    [Required(ErrorMessage = "対応期限は必須です")]
    public DateTime DueDate { get; set; }
}

/// <summary>
/// 対応開始用DTO
/// </summary>
public class StartResponseDto
{
    [Required(ErrorMessage = "インシデントIDは必須です")]
    public int IncidentId { get; set; }
}

/// <summary>
/// 原因分析用DTO
/// </summary>
public class AnalyzeCauseDto
{
    [Required(ErrorMessage = "インシデントIDは必須です")]
    public int IncidentId { get; set; }

    [Required(ErrorMessage = "原因は必須です")]
    [StringLength(2000, ErrorMessage = "原因は2000文字以内で入力してください")]
    public string Cause { get; set; } = string.Empty;
}

/// <summary>
/// 対応完了用DTO
/// </summary>
public class CompleteResponseDto
{
    [Required(ErrorMessage = "インシデントIDは必須です")]
    public int IncidentId { get; set; }

    [Required(ErrorMessage = "対応内容は必須です")]
    [StringLength(2000, ErrorMessage = "対応内容は2000文字以内で入力してください")]
    public string ResponseContent { get; set; } = string.Empty;
}

/// <summary>
/// 再発防止策提案用DTO
/// </summary>
public class ProposePreventionDto
{
    [Required(ErrorMessage = "インシデントIDは必須です")]
    public int IncidentId { get; set; }

    [Required(ErrorMessage = "再発防止策は必須です")]
    [StringLength(2000, ErrorMessage = "再発防止策は2000文字以内で入力してください")]
    public string PreventionMeasures { get; set; } = string.Empty;
}

/// <summary>
/// 有効性確認用DTO
/// </summary>
public class ConfirmEffectivenessDto
{
    [Required(ErrorMessage = "インシデントIDは必須です")]
    public int IncidentId { get; set; }

    [Required(ErrorMessage = "有効性確認状況は必須です")]
    public string EffectivenessStatus { get; set; } = string.Empty;

    [Required(ErrorMessage = "有効性確認コメントは必須です")]
    [StringLength(2000, ErrorMessage = "有効性確認コメントは2000文字以内で入力してください")]
    public string EffectivenessComment { get; set; } = string.Empty;
}

// EnableWorkflowDtoは削除 - 新仕様では常に新ワークフロー

/// <summary>
/// ワークフロー操作結果DTO
/// </summary>
public class WorkflowActionResultDto
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public IncidentStatus? NewStatus { get; set; }
    public List<string> AvailableActions { get; set; } = new();
}

/// <summary>
/// ワークフロー統計DTO
/// </summary>
public class WorkflowStatisticsDto
{
    public int UnclassifiedCount { get; set; }
    public int PendingCount { get; set; }
    public int InProgressCount { get; set; }
    public int CompletedCount { get; set; }
    public int PreventionProposedCount { get; set; }
    public int EffectivenessConfirmedCount { get; set; }
    public int TotalWithWorkflow { get; set; }
    public int TotalLegacyMode { get; set; }
}
