namespace LogisticsTroubleManagement.Domain.Entities;

public class AuditLog : BaseEntity
{
    public int? UserId { get; private set; }
    public string Action { get; private set; }
    public string TableName { get; private set; }
    public int? RecordId { get; private set; }
    public string? OldValues { get; private set; }
    public string? NewValues { get; private set; }
    public string? IpAddress { get; private set; }
    public string? UserAgent { get; private set; }

    // Navigation properties
    public virtual User? User { get; private set; }

    private AuditLog() { } // For EF Core

    public AuditLog(string action, string tableName, int? userId = null, int? recordId = null, string? oldValues = null, string? newValues = null, string? ipAddress = null, string? userAgent = null)
    {
        Action = action ?? throw new ArgumentNullException(nameof(action));
        TableName = tableName ?? throw new ArgumentNullException(nameof(tableName));
        UserId = userId;
        RecordId = recordId;
        OldValues = oldValues;
        NewValues = newValues;
        IpAddress = ipAddress;
        UserAgent = userAgent;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public static AuditLog Create(string action, string tableName, int? userId = null, int? recordId = null, string? oldValues = null, string? newValues = null, string? ipAddress = null, string? userAgent = null)
    {
        return new AuditLog(action, tableName, userId, recordId, oldValues, newValues, ipAddress, userAgent);
    }

    public static AuditLog CreateForCreate(string tableName, int? userId, int? recordId, string? newValues = null, string? ipAddress = null, string? userAgent = null)
    {
        return new AuditLog("CREATE", tableName, userId, recordId, null, newValues, ipAddress, userAgent);
    }

    public static AuditLog CreateForUpdate(string tableName, int? userId, int? recordId, string? oldValues = null, string? newValues = null, string? ipAddress = null, string? userAgent = null)
    {
        return new AuditLog("UPDATE", tableName, userId, recordId, oldValues, newValues, ipAddress, userAgent);
    }

    public static AuditLog CreateForDelete(string tableName, int? userId, int? recordId, string? oldValues = null, string? ipAddress = null, string? userAgent = null)
    {
        return new AuditLog("DELETE", tableName, userId, recordId, oldValues, null, ipAddress, userAgent);
    }

    public static AuditLog CreateForLogin(int userId, string? ipAddress = null, string? userAgent = null)
    {
        return new AuditLog("LOGIN", "Users", userId, userId, null, null, ipAddress, userAgent);
    }

    public static AuditLog CreateForLogout(int userId, string? ipAddress = null, string? userAgent = null)
    {
        return new AuditLog("LOGOUT", "Users", userId, userId, null, null, ipAddress, userAgent);
    }

    public bool IsCreateAction()
    {
        return Action.Equals("CREATE", StringComparison.OrdinalIgnoreCase);
    }

    public bool IsUpdateAction()
    {
        return Action.Equals("UPDATE", StringComparison.OrdinalIgnoreCase);
    }

    public bool IsDeleteAction()
    {
        return Action.Equals("DELETE", StringComparison.OrdinalIgnoreCase);
    }

    public bool IsLoginAction()
    {
        return Action.Equals("LOGIN", StringComparison.OrdinalIgnoreCase);
    }

    public bool IsLogoutAction()
    {
        return Action.Equals("LOGOUT", StringComparison.OrdinalIgnoreCase);
    }

    public bool HasChanges()
    {
        return !string.IsNullOrEmpty(OldValues) || !string.IsNullOrEmpty(NewValues);
    }

    public string GetActionDescription()
    {
        return Action switch
        {
            "CREATE" => "作成",
            "UPDATE" => "更新",
            "DELETE" => "削除",
            "LOGIN" => "ログイン",
            "LOGOUT" => "ログアウト",
            _ => Action
        };
    }
}
