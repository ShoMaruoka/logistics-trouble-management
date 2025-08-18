using LogisticsTroubleManagement.Domain.Entities;
using LogisticsTroubleManagement.Domain.Enums;

namespace LogisticsTroubleManagement.Domain.Services;

public class IncidentDomainService
{
    public bool CanUserAssignIncident(User user, Incident incident)
    {
        if (user == null || incident == null)
            return false;

        // 管理者とマネージャーのみがインシデントを割り当て可能
        if (!user.CanManageIncidents())
            return false;

        // 既に解決済みのインシデントは割り当て不可
        if (incident.IsResolved())
            return false;

        return true;
    }

    public bool CanUserResolveIncident(User user, Incident incident)
    {
        if (user == null || incident == null)
            return false;

        // 管理者、マネージャー、または割り当てられたユーザーのみが解決可能
        if (user.CanManageIncidents())
            return true;

        // 割り当てられたユーザーが解決可能
        if (incident.AssignedToId == user.Id)
            return true;

        return false;
    }

    public bool CanUserUpdateIncident(User user, Incident incident)
    {
        if (user == null || incident == null)
            return false;

        // 管理者とマネージャーは常に更新可能
        if (user.CanManageIncidents())
            return true;

        // 報告者が更新可能（ただし解決済みの場合は不可）
        if (incident.ReportedById == user.Id && !incident.IsResolved())
            return true;

        // 割り当てられたユーザーが更新可能
        if (incident.AssignedToId == user.Id)
            return true;

        return false;
    }

    public bool CanUserDeleteIncident(User user, Incident incident)
    {
        if (user == null || incident == null)
            return false;

        // 管理者のみが削除可能
        if (!user.CanManageUsers())
            return false;

        // 解決済みのインシデントは削除不可
        if (incident.IsResolved())
            return false;

        return true;
    }

    public Priority CalculatePriority(string category, string description)
    {
        var priority = Priority.Medium;

        // カテゴリに基づく優先度判定
        if (category.Contains("緊急", StringComparison.OrdinalIgnoreCase) ||
            category.Contains("Critical", StringComparison.OrdinalIgnoreCase))
        {
            priority = Priority.Critical;
        }
        else if (category.Contains("重要", StringComparison.OrdinalIgnoreCase) ||
                 category.Contains("High", StringComparison.OrdinalIgnoreCase))
        {
            priority = Priority.High;
        }
        else if (category.Contains("低", StringComparison.OrdinalIgnoreCase) ||
                 category.Contains("Low", StringComparison.OrdinalIgnoreCase))
        {
            priority = Priority.Low;
        }

        // 説明文に基づく優先度調整
        if (description.Contains("停止", StringComparison.OrdinalIgnoreCase) ||
            description.Contains("故障", StringComparison.OrdinalIgnoreCase) ||
            description.Contains("事故", StringComparison.OrdinalIgnoreCase))
        {
            priority = Priority.High;
        }

        return priority;
    }

    public TimeSpan CalculateExpectedResolutionTime(Priority priority, string category)
    {
        return priority switch
        {
            Priority.Critical => TimeSpan.FromHours(2),
            Priority.High => TimeSpan.FromHours(8),
            Priority.Medium => TimeSpan.FromDays(3),
            Priority.Low => TimeSpan.FromDays(7),
            _ => TimeSpan.FromDays(3)
        };
    }

    public bool IsIncidentOverdue(Incident incident, TimeSpan expectedResolutionTime)
    {
        return incident.IsOverdue(expectedResolutionTime);
    }

    public decimal CalculatePPM(IEnumerable<Incident> incidents, int totalShipments)
    {
        if (totalShipments <= 0)
            return 0;

        var resolvedIncidents = incidents.Count(i => i.IsResolved());
        return (decimal)resolvedIncidents / totalShipments * 1000000;
    }

    public TimeSpan CalculateAverageResolutionTime(IEnumerable<Incident> incidents)
    {
        var resolvedIncidents = incidents.Where(i => i.IsResolved() && i.ResolvedDate.HasValue).ToList();
        
        if (!resolvedIncidents.Any())
            return TimeSpan.Zero;

        var totalTime = resolvedIncidents.Sum(i => i.GetResolutionTime().Ticks);
        var averageTicks = totalTime / resolvedIncidents.Count;
        
        return TimeSpan.FromTicks(averageTicks);
    }
}
