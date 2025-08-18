using LogisticsTroubleManagement.Domain.Enums;
using LogisticsTroubleManagement.Domain.ValueObjects;

namespace LogisticsTroubleManagement.Domain.Entities;

public class User : BaseEntity
{
    public string Username { get; private set; }
    public Email Email { get; private set; }
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public UserRole Role { get; private set; }
    public bool IsActive { get; private set; }
    public PhoneNumber? PhoneNumber { get; private set; }

    // Navigation properties
    public virtual ICollection<Incident> ReportedIncidents { get; private set; } = new List<Incident>();
    public virtual ICollection<Incident> AssignedIncidents { get; private set; } = new List<Incident>();
    public virtual ICollection<Attachment> UploadedAttachments { get; private set; } = new List<Attachment>();
    public virtual ICollection<AuditLog> AuditLogs { get; private set; } = new List<AuditLog>();
    public virtual ICollection<Effectiveness> MeasuredEffectiveness { get; private set; } = new List<Effectiveness>();

    private User() { } // For EF Core

    public User(string username, Email email, string firstName, string lastName, UserRole role = UserRole.User)
    {
        Username = username ?? throw new ArgumentNullException(nameof(username));
        Email = email ?? throw new ArgumentNullException(nameof(email));
        FirstName = firstName ?? throw new ArgumentNullException(nameof(firstName));
        LastName = lastName ?? throw new ArgumentNullException(nameof(lastName));
        Role = role;
        IsActive = true;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public static User Create(string username, string email, string firstName, string lastName, UserRole role = UserRole.User)
    {
        var emailValueObject = Email.Create(email);
        return new User(username, emailValueObject, firstName, lastName, role);
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

    public void UpdateRole(UserRole role)
    {
        Role = role;
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

    public string GetFullName()
    {
        return $"{LastName} {FirstName}";
    }

    public bool HasPermission(UserRole requiredRole)
    {
        return Role >= requiredRole;
    }

    public bool CanManageIncidents()
    {
        return Role == UserRole.Manager || Role == UserRole.Admin;
    }

    public bool CanManageUsers()
    {
        return Role == UserRole.Admin;
    }
}
