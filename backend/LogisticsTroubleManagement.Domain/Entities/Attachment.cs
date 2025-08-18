namespace LogisticsTroubleManagement.Domain.Entities;

public class Attachment : BaseEntity
{
    public int IncidentId { get; private set; }
    public string FileName { get; private set; } = string.Empty;
    public string FilePath { get; private set; } = string.Empty;
    public long FileSize { get; private set; }
    public string ContentType { get; private set; } = string.Empty;
    public int UploadedById { get; private set; }
    public DateTime UploadedAt { get; private set; }

    // Navigation properties
    public virtual Incident Incident { get; private set; } = null!;
    public virtual User UploadedBy { get; private set; } = null!;

    private Attachment() { } // For EF Core

    public Attachment(int incidentId, string fileName, string filePath, long fileSize, string contentType, int uploadedById)
    {
        IncidentId = incidentId;
        FileName = fileName ?? throw new ArgumentNullException(nameof(fileName));
        FilePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
        FileSize = fileSize;
        ContentType = contentType ?? throw new ArgumentNullException(nameof(contentType));
        UploadedById = uploadedById;
        UploadedAt = DateTime.UtcNow;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public static Attachment Create(int incidentId, string fileName, string filePath, long fileSize, string contentType, int uploadedById)
    {
        if (fileSize <= 0)
            throw new ArgumentException("File size must be greater than 0", nameof(fileSize));

        if (string.IsNullOrWhiteSpace(fileName))
            throw new ArgumentException("File name cannot be empty", nameof(fileName));

        if (string.IsNullOrWhiteSpace(contentType))
            throw new ArgumentException("Content type cannot be empty", nameof(contentType));

        return new Attachment(incidentId, fileName, filePath, fileSize, contentType, uploadedById);
    }

    public void UpdateFileName(string newFileName)
    {
        FileName = newFileName ?? throw new ArgumentNullException(nameof(newFileName));
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateFilePath(string newFilePath)
    {
        FilePath = newFilePath ?? throw new ArgumentNullException(nameof(newFilePath));
        UpdatedAt = DateTime.UtcNow;
    }

    public string GetFileExtension()
    {
        return Path.GetExtension(FileName);
    }

    public string GetFileNameWithoutExtension()
    {
        return Path.GetFileNameWithoutExtension(FileName);
    }

    public bool IsImage()
    {
        return ContentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase);
    }

    public bool IsDocument()
    {
        var documentTypes = new[] { "application/pdf", "application/msword", "application/vnd.openxmlformats-officedocument.wordprocessingml.document" };
        return documentTypes.Any(type => ContentType.Equals(type, StringComparison.OrdinalIgnoreCase));
    }

    public string GetFormattedFileSize()
    {
        const int bytesInKB = 1024;
        const int bytesInMB = 1024 * 1024;
        const int bytesInGB = 1024 * 1024 * 1024;

        if (FileSize < bytesInKB)
            return $"{FileSize} B";
        else if (FileSize < bytesInMB)
            return $"{FileSize / bytesInKB:F1} KB";
        else if (FileSize < bytesInGB)
            return $"{FileSize / bytesInMB:F1} MB";
        else
            return $"{FileSize / bytesInGB:F1} GB";
    }

    public bool IsValidFileType()
    {
        var allowedTypes = new[]
        {
            "image/jpeg", "image/png", "image/gif", "image/bmp",
            "application/pdf",
            "application/msword",
            "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            "application/vnd.ms-excel",
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            "text/plain",
            "text/csv"
        };

        return allowedTypes.Contains(ContentType.ToLowerInvariant());
    }

    public bool IsValidFileSize(long maxFileSizeInBytes)
    {
        return FileSize <= maxFileSizeInBytes;
    }
}
