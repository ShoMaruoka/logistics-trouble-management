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
	public async Task<ActionResult<StatisticsSummaryDto>> GetSummary([FromQuery] int totalShipments = 100000)
	{
		try
		{
			_logger.LogInformation("統計サマリー取得");

			var total = (await _incidentRepository.GetAllAsync()).Count();
			var open = await _incidentRepository.GetCountByStatusAsync(IncidentStatus.Open);
			var inProgress = await _incidentRepository.GetCountByStatusAsync(IncidentStatus.InProgress);
			var resolved = await _incidentRepository.GetCountByStatusAsync(IncidentStatus.Resolved);
			var closed = await _incidentRepository.GetCountByStatusAsync(IncidentStatus.Closed);

			var critical = await _incidentRepository.GetCountByPriorityAsync(Priority.Critical);
			var high = await _incidentRepository.GetCountByPriorityAsync(Priority.High);
			var medium = await _incidentRepository.GetCountByPriorityAsync(Priority.Medium);
			var low = await _incidentRepository.GetCountByPriorityAsync(Priority.Low);

			var avg = await _incidentRepository.GetAverageResolutionTimeAsync();
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
				AverageResolutionTime = avg,
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
	public async Task<ActionResult<PieChartDataDto>> GetDamageTypesChart([FromQuery] int year = 2024)
	{
		try
		{
			_logger.LogInformation($"損傷種類別グラフデータ取得 (年度: {year})");

			var incidents = await _incidentRepository.GetAllAsync();
			var yearIncidents = incidents.Where(i => i.ReportedDate.Year == year).ToList();

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
			foreach (var incident in yearIncidents)
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

			var chartData = new PieChartDataDto
			{
				Title = $"{year}年度 損傷種類別発生件数",
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
	public async Task<ActionResult<PieChartDataDto>> GetTroubleTypesChart([FromQuery] int year = 2024)
	{
		try
		{
			_logger.LogInformation($"トラブル種類別グラフデータ取得 (年度: {year})");

			var incidents = await _incidentRepository.GetAllAsync();
			var yearIncidents = incidents.Where(i => i.ReportedDate.Year == year).ToList();

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
			foreach (var incident in yearIncidents)
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

			var chartData = new PieChartDataDto
			{
				Title = $"{year}年度 トラブル種類別発生件数",
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
	public async Task<ActionResult<ChartDataDto>> GetMonthlyIncidentsChart([FromQuery] int year = 2024)
	{
		try
		{
			_logger.LogInformation($"月別発生件数グラフデータ取得 (年度: {year})");

			var incidents = await _incidentRepository.GetAllAsync();
			var yearIncidents = incidents.Where(i => i.ReportedDate.Year == year).ToList();

			// 月別に集計
			var monthlyCounts = new int[12];
			foreach (var incident in yearIncidents)
			{
				monthlyCounts[incident.ReportedDate.Month - 1]++;
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
		catch (Exception ex)
		{
			_logger.LogError(ex, "月別発生件数グラフデータ取得中にエラー");
			return StatusCode(500, new { Error = "月別発生件数グラフデータ取得中にエラーが発生しました。" });
		}
	}
}
