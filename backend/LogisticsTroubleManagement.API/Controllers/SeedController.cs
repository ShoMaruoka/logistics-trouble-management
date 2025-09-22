using LogisticsTroubleManagement.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LogisticsTroubleManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SeedController : ControllerBase
{
	private readonly DatabaseSeeder _seeder;
	private readonly ILogger<SeedController> _logger;
	private readonly ApplicationDbContext _context;

	public SeedController(DatabaseSeeder seeder, ILogger<SeedController> logger, ApplicationDbContext context)
	{
		_seeder = seeder;
		_logger = logger;
		_context = context;
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

	[HttpPost("unset-master-data")]
	public async Task<IActionResult> SeedUnsetMasterData()
	{
		try
		{
			_logger.LogInformation("未設定マスタデータ投入のリクエストを受けました。");

			// Warehousesに未設定レコードを追加
			var existingWarehouse = await _context.Warehouses.FirstOrDefaultAsync(w => w.Id == 0);
			if (existingWarehouse == null)
			{
				await _context.Database.ExecuteSqlRawAsync(@"
					SET IDENTITY_INSERT Warehouses ON;
					INSERT INTO Warehouses (Id, Name, Location, SortOrder, IsActive, CreatedAt, UpdatedAt) 
					VALUES (0, '未設定', '未設定', 0, 1, GETDATE(), GETDATE());
					SET IDENTITY_INSERT Warehouses OFF;
				");
			}

			// ShippingCompaniesに未設定レコードを追加
			var existingShippingCompany = await _context.ShippingCompanies.FirstOrDefaultAsync(sc => sc.Id == 0);
			if (existingShippingCompany == null)
			{
				await _context.Database.ExecuteSqlRawAsync(@"
					SET IDENTITY_INSERT ShippingCompanies ON;
					INSERT INTO ShippingCompanies (Id, Name, ContactInfo, CompanyType, SortOrder, IsActive, CreatedAt, UpdatedAt) 
					VALUES (0, '未設定', '未設定', 'External', 0, 1, GETDATE(), GETDATE());
					SET IDENTITY_INSERT ShippingCompanies OFF;
				");
			}

			// TroubleTypesに未設定レコードを追加
			var existingTroubleType = await _context.TroubleTypes.FirstOrDefaultAsync(tt => tt.Id == 0);
			if (existingTroubleType == null)
			{
				await _context.Database.ExecuteSqlRawAsync(@"
					SET IDENTITY_INSERT TroubleTypes ON;
					INSERT INTO TroubleTypes (Id, Name, Description, Color, SortOrder, IsActive, CreatedAt, UpdatedAt) 
					VALUES (0, '未設定', '分類待ち', '#CCCCCC', 0, 1, GETDATE(), GETDATE());
					SET IDENTITY_INSERT TroubleTypes OFF;
				");
			}

			// DamageTypesに未設定レコードを追加
			var existingDamageType = await _context.DamageTypes.FirstOrDefaultAsync(dt => dt.Id == 0);
			if (existingDamageType == null)
			{
				await _context.Database.ExecuteSqlRawAsync(@"
					SET IDENTITY_INSERT DamageTypes ON;
					INSERT INTO DamageTypes (Id, Name, Description, Category, SortOrder, IsActive, CreatedAt, UpdatedAt) 
					VALUES (0, '未設定', '分類待ち', 'General', 0, 1, GETDATE(), GETDATE());
					SET IDENTITY_INSERT DamageTypes OFF;
				");
			}

			return Ok(new
			{
				Message = "未設定マスタデータの投入が完了しました。",
				Timestamp = DateTime.UtcNow
			});
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "未設定マスタデータ投入中にエラーが発生しました。");
			return StatusCode(500, new
			{
				Error = "未設定マスタデータの投入中にエラーが発生しました。",
				Details = ex.Message
			});
		}
	}
}
