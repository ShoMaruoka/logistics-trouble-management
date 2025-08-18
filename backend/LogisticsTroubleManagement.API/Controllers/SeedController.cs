using LogisticsTroubleManagement.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;

namespace LogisticsTroubleManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SeedController : ControllerBase
{
	private readonly DatabaseSeeder _seeder;
	private readonly ILogger<SeedController> _logger;

	public SeedController(DatabaseSeeder seeder, ILogger<SeedController> logger)
	{
		_seeder = seeder;
		_logger = logger;
	}

	[HttpPost]
	public async Task<IActionResult> SeedData()
	{
		try
		{
			_logger.LogInformation("サンプルデータ投入のリクエストを受けました。");
			
			await _seeder.SeedAsync();
			
			return Ok(new
			{
				Message = "サンプルデータの投入が完了しました。",
				Timestamp = DateTime.UtcNow
			});
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "サンプルデータ投入中にエラーが発生しました。");
			return StatusCode(500, new
			{
				Error = "サンプルデータの投入中にエラーが発生しました。",
				Details = ex.Message
			});
		}
	}

	[HttpGet("status")]
	public IActionResult GetStatus()
	{
		return Ok(new
		{
			Message = "サンプルデータ投入エンドポイントが利用可能です。",
			Endpoint = "POST /api/seed",
			Timestamp = DateTime.UtcNow
		});
	}
}
