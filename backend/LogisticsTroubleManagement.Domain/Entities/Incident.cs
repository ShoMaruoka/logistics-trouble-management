using LogisticsTroubleManagement.Domain.Enums;

namespace LogisticsTroubleManagement.Domain.Entities;

public class Incident : BaseEntity
{
    public string Title { get; private set; }
    public string Description { get; private set; }
    public IncidentStatus Status { get; private set; }
    public Priority Priority { get; private set; }
    public string Category { get; private set; }
    
    // 物流特化項目
    public int TroubleTypeId { get; private set; }
    public int DamageTypeId { get; private set; }
    public int WarehouseId { get; private set; }
    public int ShippingCompanyId { get; private set; }
    public EffectivenessStatus EffectivenessStatus { get; private set; }
    
    // 新規追加項目（提供サイト対応）
    public string IncidentDetails { get; private set; } = string.Empty; // 発生経緯
    public int TotalShipments { get; private set; } = 0; // 出荷総数
    public int DefectiveItems { get; private set; } = 0; // 不良品数
    public DateTime OccurrenceDate { get; private set; } // 発生日
    public string OccurrenceLocation { get; private set; } = string.Empty; // 発生場所
    public string Summary { get; private set; } = string.Empty; // 概要
    public string? Cause { get; private set; } // 原因
    public string? PreventionMeasures { get; private set; } // 再発防止策
    public DateTime? EffectivenessDate { get; private set; } // 有効性確認日
    public string EffectivenessComment { get; private set; } = string.Empty; // 有効性確認コメント
    public string? ClassificationNotes { get; private set; } // 分類メモ
    public DateTime? ExpectedResolutionDate { get; private set; } // 期待解決日
    
    // WorkflowStatusは削除し、Statusに統合（新仕様完全準拠）
    public DateTime? DueDate { get; private set; } // 対応期限
    
    // ワークフロー進捗日付
    public DateTime? ResponseStartDate { get; private set; } // 対応開始日
    public DateTime? CauseAnalysisDate { get; private set; } // 原因入力日
    public DateTime? CompletionDate { get; private set; } // 対応完了日
    public DateTime? PreventionProposalDate { get; private set; } // 再発防止策提案日
    public DateTime? EffectivenessConfirmationDate { get; private set; } // 有効性確認日
    
    // 新作業内容フィールド
    public string? ResponseContent { get; private set; } // 対応内容
    
    public int ReportedById { get; private set; }
    public int? AssignedToId { get; private set; }
    public DateTime ReportedDate { get; private set; }
    public DateTime? ResolvedDate { get; private set; }
    public string? Resolution { get; private set; }

    // Navigation properties
    public virtual User ReportedBy { get; private set; } = null!;
    public virtual User? AssignedTo { get; private set; }
    public virtual TroubleType TroubleType { get; private set; } = null!;
    public virtual DamageType DamageType { get; private set; } = null!;
    public virtual Warehouse Warehouse { get; private set; } = null!;
    public virtual ShippingCompany ShippingCompany { get; private set; } = null!;
    public virtual ICollection<Attachment> Attachments { get; private set; } = new List<Attachment>();
    public virtual ICollection<AuditLog> AuditLogs { get; private set; } = new List<AuditLog>();
    public virtual ICollection<Effectiveness> Effectiveness { get; private set; } = new List<Effectiveness>();

    private Incident() { } // For EF Core

    public Incident(string title, string description, string category, int reportedById, 
        int troubleTypeId, int damageTypeId, int warehouseId, int shippingCompanyId, 
        DateTime occurrenceDate, Priority priority = Priority.Medium,
        string incidentDetails = "", int totalShipments = 0, int defectiveItems = 0,
        string occurrenceLocation = "", string summary = "",
        string cause = "", string preventionMeasures = "")
    {
        Title = title ?? throw new ArgumentNullException(nameof(title));
        Description = description ?? throw new ArgumentNullException(nameof(description));
        Category = category ?? throw new ArgumentNullException(nameof(category));
        ReportedById = reportedById;
        TroubleTypeId = troubleTypeId;
        DamageTypeId = damageTypeId;
        WarehouseId = warehouseId;
        ShippingCompanyId = shippingCompanyId;
        EffectivenessStatus = EffectivenessStatus.NotImplemented;
        Priority = priority;
        Status = IncidentStatus.Unclassified; // 新仕様：未分類から開始
        
        // 新規追加項目
        IncidentDetails = incidentDetails;
        TotalShipments = totalShipments;
        DefectiveItems = defectiveItems;
        OccurrenceDate = occurrenceDate;
        OccurrenceLocation = occurrenceLocation;
        Summary = summary;
        Cause = cause;
        PreventionMeasures = preventionMeasures;
        
        ReportedDate = DateTime.UtcNow;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public static Incident Create(string title, string description, string category, int reportedById, 
        int troubleTypeId, int damageTypeId, int warehouseId, int shippingCompanyId, 
        DateTime occurrenceDate, Priority priority = Priority.Medium,
        string incidentDetails = "", int totalShipments = 0, int defectiveItems = 0,
        string occurrenceLocation = "", string summary = "",
        string cause = "", string preventionMeasures = "")
    {
        // Enum値の検証
        ValidateEnumValue(troubleTypeId, nameof(troubleTypeId), typeof(LogisticsTroubleManagement.Domain.Enums.TroubleType));
        ValidateEnumValue(damageTypeId, nameof(damageTypeId), typeof(LogisticsTroubleManagement.Domain.Enums.DamageType));
        ValidateEnumValue(warehouseId, nameof(warehouseId), typeof(LogisticsTroubleManagement.Domain.Enums.Warehouse));
        ValidateEnumValue(shippingCompanyId, nameof(shippingCompanyId), typeof(LogisticsTroubleManagement.Domain.Enums.ShippingCompany));

        return new Incident(title, description, category, reportedById, troubleTypeId, damageTypeId, 
            warehouseId, shippingCompanyId, occurrenceDate, priority, incidentDetails, totalShipments, defectiveItems,
            occurrenceLocation, summary, cause, preventionMeasures);
    }

    /// <summary>
    /// 事務員専用のインシデント作成（enum検証をスキップして新ワークフローで管理）
    /// </summary>
    public static Incident CreateForClerk(string title, string description, int reportedById, 
        string incidentDetails, DateTime occurrenceDate, string occurrenceLocation = "")
    {
        var incident = new Incident(
            title, 
            description, 
            "", // Category 空
            reportedById,
            (int)Enums.TroubleType.ProductTrouble, // デフォルト値
            (int)Enums.DamageType.None, // デフォルト値
            (int)Enums.Warehouse.None, // デフォルト値
            (int)Enums.ShippingCompany.None, // デフォルト値
            occurrenceDate,
            Priority.Medium,
            incidentDetails,
            0, // TotalShipments
            0, // DefectiveItems
            occurrenceLocation,
            "", // Summary
            "", // Cause
            ""); // PreventionMeasures

        // 新仕様：未分類状態で開始
        incident.Status = IncidentStatus.Unclassified;
        
        return incident;
    }

    private static void ValidateEnumValue(int value, string parameterName, Type enumType)
    {
        if (!Enum.IsDefined(enumType, value))
        {
            throw new ArgumentException($"Invalid value '{value}' for {parameterName}. Must be a valid {enumType.Name} enum value.", parameterName);
        }
    }

    public void AssignTo(int userId)
    {
        AssignedToId = userId;
        Status = IncidentStatus.InProgress;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// 事務員が編集可能な項目のみ更新
    /// </summary>
    public void UpdateClerkEditableFields(string title, string description, string incidentDetails, 
        DateTime occurrenceDate, string occurrenceLocation)
    {
        Title = title ?? throw new ArgumentNullException(nameof(title));
        Description = description ?? throw new ArgumentNullException(nameof(description));
        IncidentDetails = incidentDetails ?? throw new ArgumentNullException(nameof(incidentDetails));
        OccurrenceDate = occurrenceDate;
        OccurrenceLocation = occurrenceLocation ?? "";
        UpdatedAt = DateTime.UtcNow;
    }

    public void Unassign()
    {
        AssignedToId = null;
        Status = IncidentStatus.Unclassified; // 新仕様：未分類から開始
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateStatus(IncidentStatus newStatus)
    {
        if (Status == newStatus) return;

        Status = newStatus;
        
        if (newStatus == IncidentStatus.Completed || newStatus == IncidentStatus.PreventionProposed || newStatus == IncidentStatus.EffectivenessConfirmed)
        {
            ResolvedDate = DateTime.UtcNow;
        }
        else if (newStatus == IncidentStatus.Unclassified || newStatus == IncidentStatus.Pending || newStatus == IncidentStatus.InProgress)
        {
            ResolvedDate = null;
        }

        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdatePriority(Priority newPriority)
    {
        Priority = newPriority;
        UpdatedAt = DateTime.UtcNow;
    }


    public void UpdateDetails(string title, string description, string category)
    {
        Title = title ?? throw new ArgumentNullException(nameof(title));
        Description = description ?? throw new ArgumentNullException(nameof(description));
        Category = category ?? throw new ArgumentNullException(nameof(category));
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateLogisticsDetails(int troubleTypeId, int damageTypeId, 
        int warehouseId, int shippingCompanyId)
    {
        TroubleTypeId = troubleTypeId;
        DamageTypeId = damageTypeId;
        WarehouseId = warehouseId;
        ShippingCompanyId = shippingCompanyId;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateEffectivenessStatus(EffectivenessStatus status)
    {
        EffectivenessStatus = status;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateExtendedDetails(string incidentDetails, int totalShipments, int defectiveItems,
        DateTime occurrenceDate, string occurrenceLocation, string summary, string cause, 
        string preventionMeasures, DateTime? effectivenessDate, string effectivenessComment)
    {
        IncidentDetails = incidentDetails;
        TotalShipments = totalShipments;
        DefectiveItems = defectiveItems;
        OccurrenceDate = occurrenceDate;
        OccurrenceLocation = occurrenceLocation;
        Summary = summary;
        Cause = cause;
        PreventionMeasures = preventionMeasures;
        EffectivenessDate = effectivenessDate;
        EffectivenessComment = effectivenessComment;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Resolve(string resolution)
    {
        if (string.IsNullOrWhiteSpace(resolution))
            throw new ArgumentException("Resolution cannot be empty", nameof(resolution));

        Resolution = resolution;
        Status = IncidentStatus.Completed; // 新仕様：対応済
        ResolvedDate = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Close()
    {
        if (Status != IncidentStatus.Completed)
            throw new InvalidOperationException("Incident must be completed before it can be closed");

        Status = IncidentStatus.EffectivenessConfirmed; // 新仕様：有効性確認済
        UpdatedAt = DateTime.UtcNow;
    }

    public void Cancel()
    {
        Status = IncidentStatus.EffectivenessConfirmed; // 新仕様：キャンセルも有効性確認済として扱う
        UpdatedAt = DateTime.UtcNow;
    }

    public bool IsResolved()
    {
        return Status == IncidentStatus.Completed || Status == IncidentStatus.PreventionProposed || Status == IncidentStatus.EffectivenessConfirmed;
    }

    public bool IsActive()
    {
        return Status == IncidentStatus.Unclassified || Status == IncidentStatus.Pending || Status == IncidentStatus.InProgress;
    }

    public TimeSpan GetResolutionTime()
    {
        if (!ResolvedDate.HasValue)
            return TimeSpan.Zero;

        return ResolvedDate.Value - ReportedDate;
    }

    public bool IsOverdue(TimeSpan expectedResolutionTime)
    {
        if (IsResolved())
            return false;

        return DateTime.UtcNow - ReportedDate > expectedResolutionTime;
    }

    public void AddAttachment(Attachment attachment)
    {
        if (attachment == null)
            throw new ArgumentNullException(nameof(attachment));

        Attachments.Add(attachment);
    }

    public void RemoveAttachment(int attachmentId)
    {
        var attachment = Attachments.FirstOrDefault(a => a.Id == attachmentId);
        if (attachment != null)
        {
            Attachments.Remove(attachment);
        }
    }

    // テスト用メソッド - ReportedDateを設定
    public void SetReportedDate(DateTime reportedDate)
    {
        ReportedDate = reportedDate;
    }

    // 倉庫担当用メソッド - インシデントの分類
    public void UpdateCategory(string category, string? classificationNotes = null)
    {
        Category = category ?? throw new ArgumentNullException(nameof(category));
        ClassificationNotes = classificationNotes;
        UpdatedAt = DateTime.UtcNow;
    }

    // 倉庫担当用メソッド - インシデントの詳細更新
    public void UpdateDetails(string title, string description, string summary, string cause, 
        string preventionMeasures, string category, string? classificationNotes, 
        IncidentStatus status, Priority priority, DateTime? expectedResolutionDate = null)
    {
        Title = title ?? throw new ArgumentNullException(nameof(title));
        Description = description ?? throw new ArgumentNullException(nameof(description));
        Summary = summary ?? throw new ArgumentNullException(nameof(summary));
        Cause = cause ?? throw new ArgumentNullException(nameof(cause));
        PreventionMeasures = preventionMeasures ?? throw new ArgumentNullException(nameof(preventionMeasures));
        Category = category ?? throw new ArgumentNullException(nameof(category));
        ClassificationNotes = classificationNotes;
        Status = status;
        Priority = priority;
        ExpectedResolutionDate = expectedResolutionDate;
        UpdatedAt = DateTime.UtcNow;
    }

    #region 新ワークフロー関連メソッド

    // EnableNewWorkflowメソッドは削除 - Statusに統合

    /// <summary>
    /// インシデントを分類する（未分類 → 未対応）
    /// </summary>
    public void Classify(int troubleTypeId, int damageTypeId, int warehouseId, 
        int shippingCompanyId, int totalShipments, int defectiveItems, 
        Priority priority, DateTime dueDate)
    {
        if (Status != IncidentStatus.Unclassified)
            throw new InvalidOperationException("未分類状態のインシデントのみ分類可能です");
            
        TroubleTypeId = troubleTypeId;
        DamageTypeId = damageTypeId;
        WarehouseId = warehouseId;
        ShippingCompanyId = shippingCompanyId;
        TotalShipments = totalShipments;
        DefectiveItems = defectiveItems;
        Priority = priority;
        DueDate = dueDate;
        
        Status = IncidentStatus.Pending; // 新仕様：未対応
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// 対応を開始する（未対応 → 対応中）
    /// </summary>
    public void StartResponse()
    {
        if (Status != IncidentStatus.Pending)
            throw new InvalidOperationException("未対応状態のインシデントのみ対応開始可能です");
            
        ResponseStartDate = DateTime.UtcNow;
        Status = IncidentStatus.InProgress; // 新仕様：対応中
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// 原因を入力する（対応中状態を維持）
    /// </summary>
    public void AnalyzeCause(string cause)
    {
        if (Status != IncidentStatus.InProgress)
            throw new InvalidOperationException("対応中状態のインシデントのみ原因入力可能です");
            
        Cause = cause ?? throw new ArgumentNullException(nameof(cause));
        CauseAnalysisDate = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// 対応を完了する（対応中 → 対応済）
    /// </summary>
    public void CompleteResponse(string responseContent)
    {
        if (Status != IncidentStatus.InProgress)
            throw new InvalidOperationException("対応中状態のインシデントのみ対応完了可能です");
            
        ResponseContent = responseContent ?? throw new ArgumentNullException(nameof(responseContent));
        CompletionDate = DateTime.UtcNow;
        Status = IncidentStatus.Completed; // 新仕様：対応済
        ResolvedDate = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// 再発防止策を提案する（対応済 → 再発防止策提案済）
    /// </summary>
    public void ProposePreventionMeasures(string preventionMeasures)
    {
        if (Status != IncidentStatus.Completed)
            throw new InvalidOperationException("対応済状態のインシデントのみ再発防止策提案可能です");
            
        PreventionMeasures = preventionMeasures ?? throw new ArgumentNullException(nameof(preventionMeasures));
        PreventionProposalDate = DateTime.UtcNow;
        Status = IncidentStatus.PreventionProposed; // 新仕様：再発防止策提案済
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// 有効性を確認する（再発防止策提案済 → 有効性確認済）
    /// </summary>
    public void ConfirmEffectiveness(string effectivenessStatus, string effectivenessComment)
    {
        if (Status != IncidentStatus.PreventionProposed)
            throw new InvalidOperationException("再発防止策提案済状態のインシデントのみ有効性確認可能です");
            
        EffectivenessComment = effectivenessComment ?? throw new ArgumentNullException(nameof(effectivenessComment));
        EffectivenessConfirmationDate = DateTime.UtcNow;
        EffectivenessDate = DateTime.UtcNow; // 既存フィールドも更新
        
        // effectivenessStatusをEffectivenessStatus enumに変換
        if (Enum.TryParse<EffectivenessStatus>(effectivenessStatus, out var status))
        {
            EffectivenessStatus = status;
        }
        
        Status = IncidentStatus.EffectivenessConfirmed; // 新仕様：有効性確認済
        UpdatedAt = DateTime.UtcNow;
    }

    // IsWorkflowEnabledメソッドは削除 - 新仕様では常に新ワークフロー

    /// <summary>
    /// 現在のワークフローステータスに基づいて次に可能なアクションを取得
    /// </summary>
    public List<string> GetAvailableWorkflowActions(string userRole)
    {
        var actions = new List<string>();
        
        switch (Status)
        {
            case IncidentStatus.Unclassified:
                if (userRole == "Incident Manager")
                    actions.Add("classify");
                break;
                
            case IncidentStatus.Pending:
                if (userRole == "Warehouse Staff")
                    actions.Add("start-response");
                break;
                
            case IncidentStatus.InProgress:
                if (userRole == "Warehouse Staff")
                {
                    if (string.IsNullOrEmpty(Cause))
                        actions.Add("analyze-cause");
                    actions.Add("complete-response");
                }
                break;
                
            case IncidentStatus.Completed:
                if (userRole == "Warehouse Staff")
                    actions.Add("propose-prevention");
                break;
                
            case IncidentStatus.PreventionProposed:
                if (userRole == "Incident Manager")
                    actions.Add("confirm-effectiveness");
                break;
                
            case IncidentStatus.EffectivenessConfirmed:
                // 完了状態 - アクションなし
                break;
        }
        
        return actions;
    }

    #endregion
}
