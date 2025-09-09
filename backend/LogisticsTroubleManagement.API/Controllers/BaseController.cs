using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LogisticsTroubleManagement.API.Controllers;

/// <summary>
/// ベースコントローラー
/// </summary>
[ApiController]
public abstract class BaseController : ControllerBase
{
    /// <summary>
    /// 現在のユーザーIDを取得
    /// </summary>
    /// <returns>ユーザーID</returns>
    protected int GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
        {
            throw new UnauthorizedAccessException("ユーザーIDが取得できません");
        }
        return userId;
    }

    /// <summary>
    /// 現在のユーザーロールを取得
    /// </summary>
    /// <returns>ユーザーロール</returns>
    protected string GetCurrentUserRole()
    {
        var roleClaim = User.FindFirst(ClaimTypes.Role);
        return roleClaim?.Value ?? string.Empty;
    }

    /// <summary>
    /// 現在のユーザー名を取得
    /// </summary>
    /// <returns>ユーザー名</returns>
    protected string GetCurrentUsername()
    {
        var nameClaim = User.FindFirst(ClaimTypes.Name);
        return nameClaim?.Value ?? string.Empty;
    }

    /// <summary>
    /// 指定されたロールを持っているかチェック
    /// </summary>
    /// <param name="role">チェックするロール</param>
    /// <returns>ロールを持っているか</returns>
    protected bool HasRole(string role)
    {
        var userRole = GetCurrentUserRole();
        return userRole.Equals(role, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// 指定されたロールのいずれかを持っているかチェック
    /// </summary>
    /// <param name="roles">チェックするロール配列</param>
    /// <returns>いずれかのロールを持っているか</returns>
    protected bool HasAnyRole(params string[] roles)
    {
        var userRole = GetCurrentUserRole();
        return roles.Any(role => userRole.Equals(role, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// 管理者権限を持っているかチェック
    /// </summary>
    /// <returns>管理者権限を持っているか</returns>
    protected bool IsAdmin()
    {
        return HasRole("Admin");
    }

    /// <summary>
    /// インシデント管理者権限を持っているかチェック
    /// </summary>
    /// <returns>インシデント管理者権限を持っているか</returns>
    protected bool IsIncidentManager()
    {
        return HasRole("IncidentManager");
    }

    /// <summary>
    /// 倉庫担当権限を持っているかチェック
    /// </summary>
    /// <returns>倉庫担当権限を持っているか</returns>
    protected bool IsWarehouseStaff()
    {
        return HasRole("WarehouseStaff");
    }

    /// <summary>
    /// 事務員権限を持っているかチェック
    /// </summary>
    /// <returns>事務員権限を持っているか</returns>
    protected bool IsClerk()
    {
        return HasRole("Clerk");
    }
}
