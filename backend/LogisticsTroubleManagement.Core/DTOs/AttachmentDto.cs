namespace LogisticsTroubleManagement.Core.DTOs;

public class AttachmentDto
{
    public int Id { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public DateTime UploadedAt { get; set; }
    public int IncidentId { get; set; }
    public string IncidentTitle { get; set; } = string.Empty;
    public string UploadedBy { get; set; } = string.Empty;
}

public class CreateAttachmentDto
{
    public int IncidentId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public int UploadedById { get; set; }
}

public class AttachmentSearchDto
{
    public int? IncidentId { get; set; }
    public string? FileName { get; set; }
    public string? ContentType { get; set; }
    public int? UploadedById { get; set; }
    public DateTime? UploadedFrom { get; set; }
    public DateTime? UploadedTo { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SortBy { get; set; } = "UploadedAt";
    public bool Ascending { get; set; } = false;
}
