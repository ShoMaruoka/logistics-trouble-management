namespace LogisticsTroubleManagement.Core.DTOs;

public class StatisticsSummaryDto
{
	public int TotalIncidents { get; set; }
	public int OpenCount { get; set; }
	public int InProgressCount { get; set; }
	public int ResolvedCount { get; set; }
	public int ClosedCount { get; set; }
	public int CriticalCount { get; set; }
	public int HighCount { get; set; }
	public int MediumCount { get; set; }
	public int LowCount { get; set; }
	public TimeSpan AverageResolutionTime { get; set; }
	public decimal Ppm { get; set; }
}
