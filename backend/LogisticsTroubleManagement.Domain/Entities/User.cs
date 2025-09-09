using LogisticsTroubleManagement.Domain.Enums;
using LogisticsTroubleManagement.Domain.ValueObjects;

namespace LogisticsTroubleManagement.Domain.Entities;

public class User : BaseEntity
{
    public string Username { get; private set; }
    public Email Email { get; private set; }
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public bool IsActive { get; private set; }
    public PhoneNumber? PhoneNumber { get; private set; }
    
    // 認証関連プロパティ
    public string PasswordHash { get; private set; } = string.Empty;
    public DateTime? LastLoginAt { get; private set; }
    public DateTime? LastPasswordChangeAt { get; private set; }
    public int TokenVersion { get; private set; } = 1;
    public int RoleId { get; private set; }

    // Navigation properties
    public virtual ICollection<Incident> ReportedIncidents { get; private set; } = new List<Incident>();
    public virtual ICollection<Incident> AssignedIncidents { get; private set; } = new List<Incident>();
    public virtual ICollection<Attachment> UploadedAttachments { get; private set; } = new List<Attachment>();
    public virtual ICollection<AuditLog> AuditLogs { get; private set; } = new List<AuditLog>();
    public virtual ICollection<Effectiveness> MeasuredEffectiveness { get; private set; } = new List<Effectiveness>();
    public virtual ICollection<RefreshToken> RefreshTokens { get; private set; } = new List<RefreshToken>();
    public virtual Role Role { get; private set; } = null!;

    private User() { } // For EF Core

    public User(string username, Email email, string firstName, string lastName, int roleId)
    {
        Username = username ?? throw new ArgumentNullException(nameof(username));
        Email = email ?? throw new ArgumentNullException(nameof(email));
        FirstName = firstName ?? throw new ArgumentNullException(nameof(firstName));
        LastName = lastName ?? throw new ArgumentNullException(nameof(lastName));
        RoleId = roleId;
        IsActive = true;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public static User Create(string username, string email, string firstName, string lastName, int roleId)
    {
        var emailValueObject = Email.Create(email);
        return new User(username, emailValueObject, firstName, lastName, roleId);
    }

    public void UpdateProfile(string firstName, string lastName, string? phoneNumber = null)
    {
        FirstName = firstName ?? throw new ArgumentNullException(nameof(firstName));
        LastName = lastName ?? throw new ArgumentNullException(nameof(lastName));
        
        if (!string.IsNullOrWhiteSpace(phoneNumber))
        {
            PhoneNumber = PhoneNumber.Create(phoneNumber);
        }
        
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateEmail(string email)
    {
        Email = Email.Create(email);
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateRole(int roleId)
    {
        RoleId = roleId;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetRole(Role role)
    {
        Role = role ?? throw new ArgumentNullException(nameof(role));
        RoleId = role.Id;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Activate()
    {
        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateLastLoginAt()
    {
        LastLoginAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateLastPasswordChangeAt()
    {
        LastPasswordChangeAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public string GetFullName()
    {
        return $"{LastName} {FirstName}";
    }

    public bool HasRole(int roleId)
    {
        return RoleId == roleId;
    }

    public bool HasRole(string roleName)
    {
        return Role?.Name == roleName;
    }

    public bool CanManageIncidents()
    {
        return HasRole("Incident Manager") || HasRole("Admin");
    }

    public bool CanManageUsers()
    {
        return HasRole("Admin");
    }

    // 認証関連メソッド
    public void SetPasswordHash(string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new ArgumentException("Password hash cannot be empty", nameof(passwordHash));

        PasswordHash = passwordHash;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateLastLogin()
    {
        LastLoginAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void IncrementTokenVersion()
    {
        TokenVersion++;
        UpdatedAt = DateTime.UtcNow;
    }

    public void RevokeAllRefreshTokens()
    {
        foreach (var token in RefreshTokens.Where(t => t.IsActive))
        {
            token.Revoke("User logout", "User requested logout");
        }
        IncrementTokenVersion();
    }

    // ユーザー管理用メソッド
    public void UpdateUserInfo(string username, string email, string firstName, string lastName)
    {
        if (string.IsNullOrWhiteSpace(username))
            throw new ArgumentException("Username cannot be empty", nameof(username));
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email cannot be empty", nameof(email));
        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentException("First name cannot be empty", nameof(firstName));
        if (string.IsNullOrWhiteSpace(lastName))
            throw new ArgumentException("Last name cannot be empty", nameof(lastName));

        Username = username;
        Email = Email.Create(email);
        FirstName = firstName;
        LastName = lastName;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetActiveStatus(bool isActive)
    {
        IsActive = isActive;
        UpdatedAt = DateTime.UtcNow;
    }
}
