using LogisticsTroubleManagement.Domain.Entities;
using LogisticsTroubleManagement.Domain.Enums;
using LogisticsTroubleManagement.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LogisticsTroubleManagement.Infrastructure.Data;

public class DatabaseSeeder
{
	private readonly ApplicationDbContext _context;
	private readonly ILogger<DatabaseSeeder> _logger;

	public DatabaseSeeder(ApplicationDbContext context, ILogger<DatabaseSeeder> logger)
	{
		_context = context;
		_logger = logger;
	}

	public async Task SeedAsync()
	{
		try
		{
			_logger.LogInformation("サンプルデータの投入を開始します...");

			// ユーザーデータの投入
			await SeedUsersAsync();
			await _context.SaveChangesAsync();

			// インシデントデータの投入
			await SeedIncidentsAsync();
			await _context.SaveChangesAsync();

			// 添付ファイルデータの投入
			await SeedAttachmentsAsync();
			await _context.SaveChangesAsync();

			// 監査ログデータの投入
			await SeedAuditLogsAsync();
			await _context.SaveChangesAsync();

			// 効果測定データの投入
			await SeedEffectivenessAsync();
			await _context.SaveChangesAsync();

			_logger.LogInformation("サンプルデータの投入が完了しました。");
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "サンプルデータの投入中にエラーが発生しました。");
			throw;
		}
	}

	private async Task SeedUsersAsync()
	{
		if (await _context.Users.AnyAsync())
		{
			_logger.LogInformation("ユーザーデータは既に存在するため、スキップします。");
			return;
		}

		var users = new List<User>
		{
			User.Create("admin", "admin@example.com", "管理者", "太郎", UserRole.Admin),
			User.Create("manager1", "manager1@example.com", "マネージャー", "花子", UserRole.Manager),
			User.Create("manager2", "manager2@example.com", "マネージャー", "次郎", UserRole.Manager),
			User.Create("user1", "user1@example.com", "一般", "三郎", UserRole.User),
			User.Create("user2", "user2@example.com", "一般", "四郎", UserRole.User),
			User.Create("user3", "user3@example.com", "一般", "五郎", UserRole.User)
		};

		// 電話番号を設定
		users[0].UpdateProfile("管理者", "太郎", "03-1234-5678");
		users[1].UpdateProfile("マネージャー", "花子", "03-2345-6789");
		users[2].UpdateProfile("マネージャー", "次郎", "090-1234-5678");

		await _context.Users.AddRangeAsync(users);
		_logger.LogInformation("{Count}件のユーザーデータを追加しました。", users.Count);
	}

	private async Task SeedIncidentsAsync()
	{
		if (await _context.Incidents.AnyAsync())
		{
			_logger.LogInformation("インシデントデータは既に存在するため、スキップします。");
			return;
		}

		var users = await _context.Users.ToListAsync();
		if (!users.Any())
		{
			_logger.LogWarning("ユーザーデータが存在しないため、インシデントデータの投入をスキップします。");
			return;
		}

		var incidents = new List<Incident>
		{
			Incident.Create(
				"配送車両の故障",
				"配送車両A-001がエンジンオイル漏れのため、緊急点検が必要です。",
				"車両故障",
				users[3].Id, // user1
				TroubleType.DeliveryTrouble,
				DamageType.OtherDeliveryMistake,
				Warehouse.WarehouseA,
				ShippingCompany.ATransport,
				new DateTime(2025, 8, 15), // 発生日
				Priority.High
			),
			Incident.Create(
				"倉庫の温度管理システム異常",
				"冷蔵倉庫の温度が設定値より2度高い状態が続いています。",
				"設備故障",
				users[4].Id, // user2
				TroubleType.ProductTrouble,
				DamageType.OtherProductAccident,
				Warehouse.WarehouseB,
				ShippingCompany.InHouse,
				new DateTime(2025, 8, 16), // 発生日
				Priority.Critical
			),
			Incident.Create(
				"配送遅延の報告",
				"関東地区の配送が台風の影響で1日遅延する見込みです。",
				"配送遅延",
				users[5].Id, // user3
				TroubleType.DeliveryTrouble,
				DamageType.EarlyOrLateArrival,
				Warehouse.WarehouseB,
				ShippingCompany.Charter,
				new DateTime(2025, 8, 17), // 発生日
				Priority.Medium
			),
			Incident.Create(
				"荷物の破損",
				"精密機器の荷物が輸送中に破損しました。",
				"荷物破損",
				users[3].Id, // user1
				TroubleType.ProductTrouble,
				DamageType.DamageOrContamination,
				Warehouse.WarehouseA,
				ShippingCompany.BExpress,
				new DateTime(2025, 8, 18), // 発生日
				Priority.High
			),
			Incident.Create(
				"システムログインエラー",
				"複数のユーザーからログインできないとの報告があります。",
				"システム障害",
				users[4].Id, // user2
				TroubleType.ProductTrouble,
				DamageType.OtherProductAccident,
				Warehouse.WarehouseC,
				ShippingCompany.ATransport,
				new DateTime(2025, 8, 19), // 発生日
				Priority.Critical
			)
		};

		// 一部のインシデントを割り当て・解決済みにする
		incidents[0].AssignTo(users[1].Id); // manager1に割り当て
		incidents[1].AssignTo(users[2].Id); // manager2に割り当て
		incidents[2].AssignTo(users[1].Id); // manager1に割り当て

		// 解決済みにする
		incidents[0].Resolve("エンジンオイルを交換し、配管の接続部分を修理しました。");
		incidents[0].Close();

		incidents[1].Resolve("温度センサーを交換し、設定値を調整しました。");
		incidents[1].Close();

		await _context.Incidents.AddRangeAsync(incidents);
		_logger.LogInformation("{Count}件のインシデントデータを追加しました。", incidents.Count);
	}

	private async Task SeedAttachmentsAsync()
	{
		if (await _context.Attachments.AnyAsync())
		{
			_logger.LogInformation("添付ファイルデータは既に存在するため、スキップします。");
			return;
		}

		var incidents = await _context.Incidents.ToListAsync();
		var users = await _context.Users.ToListAsync();

		if (!incidents.Any() || !users.Any())
		{
			_logger.LogWarning("インシデントまたはユーザーデータが存在しないため、添付ファイルデータの投入をスキップします。");
			return;
		}

		var attachments = new List<Attachment>
		{
			Attachment.Create(
				incidents[0].Id,
				"vehicle_damage.jpg",
				"/uploads/incidents/vehicle_damage.jpg",
				1024000, // 1MB
				"image/jpeg",
				users[3].Id // user1
			),
			Attachment.Create(
				incidents[1].Id,
				"temperature_log.csv",
				"/uploads/incidents/temperature_log.csv",
				51200, // 50KB
				"text/csv",
				users[4].Id // user2
			),
			Attachment.Create(
				incidents[2].Id,
				"weather_report.pdf",
				"/uploads/incidents/weather_report.pdf",
				204800, // 200KB
				"application/pdf",
				users[5].Id // user3
			)
		};

		await _context.Attachments.AddRangeAsync(attachments);
		_logger.LogInformation("{Count}件の添付ファイルデータを追加しました。", attachments.Count);
	}

	private async Task SeedAuditLogsAsync()
	{
		if (await _context.AuditLogs.AnyAsync())
		{
			_logger.LogInformation("監査ログデータは既に存在するため、スキップします。");
			return;
		}

		var users = await _context.Users.ToListAsync();
		var incidents = await _context.Incidents.ToListAsync();

		if (!users.Any())
		{
			_logger.LogWarning("ユーザーデータが存在しないため、監査ログデータの投入をスキップします。");
			return;
		}

		var auditLogs = new List<AuditLog>
		{
			AuditLog.CreateForLogin(users[0].Id, "192.168.1.100", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36"),
			AuditLog.CreateForCreate("Incidents", users[3].Id, incidents[0].Id, "{\"Title\":\"配送車両の故障\",\"Priority\":\"High\"}"),
			AuditLog.CreateForUpdate("Incidents", users[1].Id, incidents[0].Id, "{\"Status\":\"Open\"}", "{\"Status\":\"InProgress\"}"),
			AuditLog.CreateForUpdate("Incidents", users[1].Id, incidents[0].Id, "{\"Status\":\"InProgress\"}", "{\"Status\":\"Resolved\"}"),
			AuditLog.CreateForLogin(users[1].Id, "192.168.1.101", "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_7) AppleWebKit/537.36"),
			AuditLog.CreateForLogout(users[0].Id, "192.168.1.100", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36")
		};

		await _context.AuditLogs.AddRangeAsync(auditLogs);
		_logger.LogInformation("{Count}件の監査ログデータを追加しました。", auditLogs.Count);
	}

	private async Task SeedEffectivenessAsync()
	{
		if (await _context.Effectiveness.AnyAsync())
		{
			_logger.LogInformation("効果測定データは既に存在するため、スキップします。");
			return;
		}

		var incidents = await _context.Incidents.ToListAsync();
		var resolvedIncidents = incidents.Where(i => i.IsResolved()).ToList();
		var users = await _context.Users.ToListAsync();

		if (!resolvedIncidents.Any() || !users.Any())
		{
			_logger.LogWarning("解決済みインシデントまたはユーザーデータが存在しないため、効果測定データの投入をスキップします。");
			return;
		}

		var effectiveness = new List<Effectiveness>
		{
			Effectiveness.Create(
				resolvedIncidents[0].Id,
				"COST_REDUCTION",
				100000m, // BeforeValue
				50000m,  // AfterValue
				50.0m,   // ImprovementRate (50%削減)
				"修理費用の削減",
				users[1].Id // manager1
			),
			Effectiveness.Create(
				resolvedIncidents[0].Id,
				"TIME_SAVING",
				8m,      // BeforeValue
				4m,      // AfterValue
				50.0m,   // ImprovementRate (50%短縮)
				"修理時間の短縮",
				users[1].Id // manager1
			)
		};

		// 2つ目の解決済みインシデントがある場合のみ追加
		if (resolvedIncidents.Count > 1)
		{
			effectiveness.AddRange(new[]
			{
				Effectiveness.Create(
					resolvedIncidents[1].Id,
					"QUALITY_IMPROVEMENT",
					85m,     // BeforeValue
					95m,     // AfterValue
					11.76m,  // ImprovementRate (約11.76%向上)
					"温度管理精度の向上",
					users[2].Id // manager2
				),
				Effectiveness.Create(
					resolvedIncidents[1].Id,
					"SAFETY",
					90m,     // BeforeValue
					100m,    // AfterValue
					11.11m,  // ImprovementRate (約11.11%向上)
					"食品安全性の確保",
					users[2].Id // manager2
				)
			});
		}

		await _context.Effectiveness.AddRangeAsync(effectiveness);
		_logger.LogInformation("{Count}件の効果測定データを追加しました。", effectiveness.Count);
	}
}
