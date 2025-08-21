using LogisticsTroubleManagement.Core.DTOs;
using LogisticsTroubleManagement.Domain.Enums;
using LogisticsTroubleManagement.Domain.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace LogisticsTroubleManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StatisticsController : ControllerBase
{
	private readonly IIncidentRepository _incidentRepository;
	private readonly IUnitOfWork _unitOfWork;
	private readonly ILogger<StatisticsController> _logger;

	public StatisticsController(IIncidentRepository incidentRepository, IUnitOfWork unitOfWork, ILogger<StatisticsController> logger)
	{
		_incidentRepository = incidentRepository;
		_unitOfWork = unitOfWork;
		_logger = logger;
	}

	// GET: api/statistics/summary
	[HttpGet("summary")]
	public async Task<ActionResult<StatisticsSummaryDto>> GetSummary([FromQuery] int year = 0, [FromQuery] int month = 0, [FromQuery] int totalShipments = 100000)
	{
		try
		{
					_logger.LogInformation($"統計サマリー取得 (年度: {year}, 月: {month})");

		var incidents = await _incidentRepository.GetAllAsync();
		
		// 年度・月フィルタ適用（発生日ベース）
		if (year > 0)
		{
			incidents = incidents.Where(i => i.OccurrenceDate.Year == year).ToList();
		}
		if (month > 0)
		{
			incidents = incidents.Where(i => i.OccurrenceDate.Month == month).ToList();
		}

			var total = incidents.Count();
			var open = incidents.Count(i => i.Status == IncidentStatus.Open);
			var inProgress = incidents.Count(i => i.Status == IncidentStatus.InProgress);
			var resolved = incidents.Count(i => i.Status == IncidentStatus.Resolved);
			var closed = incidents.Count(i => i.Status == IncidentStatus.Closed);

			var critical = incidents.Count(i => i.Priority == Priority.Critical);
			var high = incidents.Count(i => i.Priority == Priority.High);
			var medium = incidents.Count(i => i.Priority == Priority.Medium);
			var low = incidents.Count(i => i.Priority == Priority.Low);

			// 平均解決時間の計算（解決済み・クローズ済みのインシデントのみ）
			var resolvedIncidents = incidents.Where(i => i.Status == IncidentStatus.Resolved || i.Status == IncidentStatus.Closed).ToList();
			var avg = resolvedIncidents.Any() ? resolvedIncidents.Average(i => ((i.ResolvedDate ?? i.UpdatedAt) - i.ReportedDate).TotalDays) : 0;
			
			var ppm = await _incidentRepository.GetPPMAsync(totalShipments);

			var dto = new StatisticsSummaryDto
			{
				TotalIncidents = total,
				OpenCount = open,
				InProgressCount = inProgress,
				ResolvedCount = resolved,
				ClosedCount = closed,
				CriticalCount = critical,
				HighCount = high,
				MediumCount = medium,
				LowCount = low,
				AverageResolutionTime = Math.Round(avg, 1),
				Ppm = ppm
			};

			return Ok(dto);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "統計サマリー取得中にエラー");
			return StatusCode(500, new { Error = "統計サマリー取得中にエラーが発生しました。" });
		}
	}

	// GET: api/statistics/charts/damage-types
	[HttpGet("charts/damage-types")]
	public async Task<ActionResult<PieChartDataDto>> GetDamageTypesChart([FromQuery] int year = 0, [FromQuery] int month = 0)
	{
		try
		{
					_logger.LogInformation($"損傷種類別グラフデータ取得 (年度: {year}, 月: {month})");

		var incidents = await _incidentRepository.GetAllAsync();
		
		// 年度・月フィルタ適用（発生日ベース）
		if (year > 0)
		{
			incidents = incidents.Where(i => i.OccurrenceDate.Year == year).ToList();
		}
		if (month > 0)
		{
			incidents = incidents.Where(i => i.OccurrenceDate.Month == month).ToList();
		}

			// 損傷の種類別に集計（モックデータ）
			var damageTypes = new Dictionary<string, int>
			{
				{ "破損", 0 },
				{ "汚損", 0 },
				{ "紛失", 0 },
				{ "遅配", 0 },
				{ "その他", 0 }
			};

			// サンプルデータに基づいて集計（実際のシステムではCategoryフィールドを使用）
			foreach (var incident in incidents)
			{
				var category = incident.Category;
				if (damageTypes.ContainsKey(category))
				{
					damageTypes[category]++;
				}
				else
				{
					damageTypes["その他"]++;
				}
			}

			var colors = new[] { "#FF6384", "#36A2EB", "#FFCE56", "#4BC0C0", "#9966FF" };
			var items = damageTypes.Select((kvp, index) => new PieChartItemDto
			{
				Label = kvp.Key,
				Value = kvp.Value,
				Color = colors[index % colors.Length]
			}).ToList();

			var title = month > 0 ? $"{year}年{month}月 損傷種類別発生件数" : $"{year}年度 損傷種類別発生件数";
			var chartData = new PieChartDataDto
			{
				Title = title,
				Items = items
			};

			return Ok(chartData);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "損傷種類別グラフデータ取得中にエラー");
			return StatusCode(500, new { Error = "損傷種類別グラフデータ取得中にエラーが発生しました。" });
		}
	}

	// GET: api/statistics/charts/trouble-types
	[HttpGet("charts/trouble-types")]
	public async Task<ActionResult<PieChartDataDto>> GetTroubleTypesChart([FromQuery] int year = 0, [FromQuery] int month = 0)
	{
		try
		{
					_logger.LogInformation($"トラブル種類別グラフデータ取得 (年度: {year}, 月: {month})");

		var incidents = await _incidentRepository.GetAllAsync();
		
		// 年度・月フィルタ適用（発生日ベース）
		if (year > 0)
		{
			incidents = incidents.Where(i => i.OccurrenceDate.Year == year).ToList();
		}
		if (month > 0)
		{
			incidents = incidents.Where(i => i.OccurrenceDate.Month == month).ToList();
		}

			// トラブル種類別に集計（モックデータ）
			var troubleTypes = new Dictionary<string, int>
			{
				{ "配送トラブル", 0 },
				{ "梱包トラブル", 0 },
				{ "システムトラブル", 0 },
				{ "人的ミス", 0 },
				{ "その他", 0 }
			};

			// サンプルデータに基づいて集計（実際のシステムでは専用フィールドを使用）
			foreach (var incident in incidents)
			{
				// モックデータでは、IDの末尾で種類を判定
				var troubleType = (incident.Id % 5) switch
				{
					0 => "配送トラブル",
					1 => "梱包トラブル",
					2 => "システムトラブル",
					3 => "人的ミス",
					_ => "その他"
				};

				if (troubleTypes.ContainsKey(troubleType))
				{
					troubleTypes[troubleType]++;
				}
			}

			var colors = new[] { "#FF6384", "#36A2EB", "#FFCE56", "#4BC0C0", "#9966FF" };
			var items = troubleTypes.Select((kvp, index) => new PieChartItemDto
			{
				Label = kvp.Key,
				Value = kvp.Value,
				Color = colors[index % colors.Length]
			}).ToList();

			var title = month > 0 ? $"{year}年{month}月 トラブル種類別発生件数" : $"{year}年度 トラブル種類別発生件数";
			var chartData = new PieChartDataDto
			{
				Title = title,
				Items = items
			};

			return Ok(chartData);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "トラブル種類別グラフデータ取得中にエラー");
			return StatusCode(500, new { Error = "トラブル種類別グラフデータ取得中にエラーが発生しました。" });
		}
	}

	// GET: api/statistics/charts/monthly-incidents
	[HttpGet("charts/monthly-incidents")]
	public async Task<ActionResult<ChartDataDto>> GetMonthlyIncidentsChart([FromQuery] int year = 0, [FromQuery] int month = 0)
	{
		try
		{
					_logger.LogInformation($"月別発生件数グラフデータ取得 (年度: {year}, 月: {month})");

		var incidents = await _incidentRepository.GetAllAsync();
		
		// 年度フィルタ適用（月間表示の場合は年度のみ）（発生日ベース）
		if (year > 0)
		{
			incidents = incidents.Where(i => i.OccurrenceDate.Year == year).ToList();
		}

		// 月間表示の場合は日別集計、年間表示の場合は月別集計
		if (month > 0)
		{
			// 指定月の日別集計（発生日ベース）
			var monthIncidents = incidents.Where(i => i.OccurrenceDate.Month == month).ToList();
			var dailyCounts = new int[DateTime.DaysInMonth(year, month)];
			
			foreach (var incident in monthIncidents)
			{
				dailyCounts[incident.OccurrenceDate.Day - 1]++;
			}

				var labels = Enumerable.Range(1, DateTime.DaysInMonth(year, month)).Select(d => $"{d}日").ToList();
				var data = dailyCounts.Select(c => (decimal)c).ToList();

				var chartData = new ChartDataDto
				{
					ChartType = "bar",
					Title = $"{year}年{month}月 日別発生件数",
					Labels = labels,
					Series = new List<ChartSeriesDto>
					{
						new ChartSeriesDto
						{
							Name = "発生件数",
							Data = data
						}
					}
				};

				return Ok(chartData);
			}
			else
			{
							// 年間表示の場合は月別集計（発生日ベース）
			var monthlyCounts = new int[12];
			foreach (var incident in incidents)
			{
				monthlyCounts[incident.OccurrenceDate.Month - 1]++;
			}

				var labels = Enumerable.Range(1, 12).Select(m => $"{m}月").ToList();
				var data = monthlyCounts.Select(c => (decimal)c).ToList();

				var chartData = new ChartDataDto
				{
					ChartType = "bar",
					Title = $"{year}年度 月別発生件数",
					Labels = labels,
					Series = new List<ChartSeriesDto>
					{
						new ChartSeriesDto
						{
							Name = "発生件数",
							Data = data
						}
					}
				};

				return Ok(chartData);
			}
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "月別発生件数グラフデータ取得中にエラー");
			return StatusCode(500, new { Error = "月別発生件数グラフデータ取得中にエラーが発生しました。" });
		}
	}
}
