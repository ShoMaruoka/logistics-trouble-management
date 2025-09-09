using LogisticsTroubleManagement.Domain.Entities;
using LogisticsTroubleManagement.Domain.Enums;
using LogisticsTroubleManagement.Domain.ValueObjects;
using LogisticsTroubleManagement.Core.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LogisticsTroubleManagement.Infrastructure.Data;

public class DatabaseSeeder
{
	private readonly ApplicationDbContext _context;
	private readonly ILogger<DatabaseSeeder> _logger;
	private readonly IPasswordService _passwordService;

	public DatabaseSeeder(ApplicationDbContext context, ILogger<DatabaseSeeder> logger, IPasswordService passwordService)
	{
		_context = context;
		_logger = logger;
		_passwordService = passwordService;
	}

	public async Task SeedAsync()
	{
		try
		{
			_logger.LogInformation("サンプルデータの投入を開始します...");

			// マスタデータの投入（インシデントデータより先に）
			await SeedMasterDataAsync();
			await _context.SaveChangesAsync();

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

		// ロールの作成（先に作成）
		var roles = new List<Role>
		{
			Role.Create("Admin", "システム管理者"),
			Role.Create("Incident Manager", "インシデント管理者"),
			Role.Create("Warehouse Staff", "倉庫スタッフ"),
			Role.Create("Clerk", "一般職員")
		};

		await _context.Roles.AddRangeAsync(roles);
		await _context.SaveChangesAsync();

		// ユーザーの作成（ロールIDを指定）
		var users = new List<User>
		{
			User.Create("admin", "admin@example.com", "管理者", "太郎", roles[0].Id), // Admin
			User.Create("manager1", "manager1@example.com", "マネージャー", "花子", roles[1].Id), // Incident Manager
			User.Create("manager2", "manager2@example.com", "マネージャー", "次郎", roles[1].Id), // Incident Manager
			User.Create("user1", "user1@example.com", "一般", "三郎", roles[3].Id), // Clerk
			User.Create("user2", "user2@example.com", "一般", "四郎", roles[3].Id), // Clerk
			User.Create("user3", "user3@example.com", "一般", "五郎", roles[3].Id) // Clerk
		};

		// 電話番号を設定
		users[0].UpdateProfile("管理者", "太郎", "03-1234-5678");
		users[1].UpdateProfile("マネージャー", "花子", "03-2345-6789");
		users[2].UpdateProfile("マネージャー", "次郎", "090-1234-5678");

		// パスワードハッシュを設定
		foreach (var user in users)
		{
			var hashedPassword = _passwordService.HashPassword("password123");
			user.SetPasswordHash(hashedPassword);
		}

		await _context.Users.AddRangeAsync(users);
		await _context.SaveChangesAsync();

		_logger.LogInformation("{Count}件のユーザーデータとロールデータを追加しました。", users.Count);
	}

	private async Task SeedIncidentsAsync()
	{
		if (await _context.Incidents.AnyAsync())
		{
			_logger.LogInformation("インシデントデータは既に存在するため、スキップします。");
			return;
		}

		var users = await _context.Users.ToListAsync();
		var troubleTypes = await _context.TroubleTypes.ToListAsync();
		var damageTypes = await _context.DamageTypes.ToListAsync();
		var warehouses = await _context.Warehouses.ToListAsync();
		var shippingCompanies = await _context.ShippingCompanies.ToListAsync();

		if (!users.Any() || !troubleTypes.Any() || !damageTypes.Any() || !warehouses.Any() || !shippingCompanies.Any())
		{
			_logger.LogWarning("必要なマスタデータが存在しないため、インシデントデータの投入をスキップします。");
			return;
		}

		var incidents = new List<Incident>
		{
			Incident.Create(
				"配送車両の故障",
				"配送車両A-001がエンジンオイル漏れのため、緊急点検が必要です。",
				"車両故障",
				users[3].Id, // user1
				troubleTypes[0].Id, // 破損
				damageTypes[0].Id, // 物理的破損
				warehouses[0].Id, // 東京倉庫
				shippingCompanies[0].Id, // ヤマト運輸
				new DateTime(2025, 8, 15), // 発生日
				Priority.High
			),
			Incident.Create(
				"倉庫の温度管理システム異常",
				"冷蔵倉庫の温度が設定値より2度高い状態が続いています。",
				"設備故障",
				users[4].Id, // user2
				troubleTypes[1].Id, // 遅延
				damageTypes[2].Id, // 温度による損傷
				warehouses[1].Id, // 大阪倉庫
				shippingCompanies[1].Id, // 佐川急便
				new DateTime(2025, 8, 16), // 発生日
				Priority.Critical
			),
			Incident.Create(
				"配送遅延の報告",
				"関東地区の配送が台風の影響で1日遅延する見込みです。",
				"配送遅延",
				users[5].Id, // user3
				troubleTypes[1].Id, // 遅延
				damageTypes[0].Id, // 物理的破損
				warehouses[1].Id, // 大阪倉庫
				shippingCompanies[2].Id, // 日本通運
				new DateTime(2025, 8, 17), // 発生日
				Priority.Medium
			),
			Incident.Create(
				"荷物の破損",
				"精密機器の荷物が輸送中に破損しました。",
				"荷物破損",
				users[3].Id, // user1
				troubleTypes[0].Id, // 破損
				damageTypes[0].Id, // 物理的破損
				warehouses[0].Id, // 東京倉庫
				shippingCompanies[3].Id, // 福山通運
				new DateTime(2025, 8, 18), // 発生日
				Priority.High
			),
			Incident.Create(
				"システムログインエラー",
				"複数のユーザーからログインできないとの報告があります。",
				"システム障害",
				users[4].Id, // user2
				troubleTypes[4].Id, // 品質不良
				damageTypes[3].Id, // 汚損
				warehouses[2].Id, // 名古屋倉庫
				shippingCompanies[0].Id, // ヤマト運輸
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

	private async Task SeedMasterDataAsync()
	{
		// TroubleTypesの投入
		if (!await _context.TroubleTypes.AnyAsync())
		{
			var troubleTypes = new List<LogisticsTroubleManagement.Domain.Entities.TroubleType>
			{
				new LogisticsTroubleManagement.Domain.Entities.TroubleType("破損", "商品の破損に関するトラブル", "#FF6B6B"),
				new LogisticsTroubleManagement.Domain.Entities.TroubleType("遅延", "配送遅延に関するトラブル", "#4ECDC4"),
				new LogisticsTroubleManagement.Domain.Entities.TroubleType("紛失", "商品紛失に関するトラブル", "#45B7D1"),
				new LogisticsTroubleManagement.Domain.Entities.TroubleType("誤配送", "配送先間違いに関するトラブル", "#96CEB4"),
				new LogisticsTroubleManagement.Domain.Entities.TroubleType("品質不良", "商品品質に関するトラブル", "#FFEAA7")
			};

			await _context.TroubleTypes.AddRangeAsync(troubleTypes);
			_logger.LogInformation("{Count}件のトラブルタイプデータを追加しました。", troubleTypes.Count);
		}

		// DamageTypesの投入
		if (!await _context.DamageTypes.AnyAsync())
		{
			var damageTypes = new List<LogisticsTroubleManagement.Domain.Entities.DamageType>
			{
				new LogisticsTroubleManagement.Domain.Entities.DamageType("物理的破損", "PHYSICAL", "外箱の破損、商品の変形など"),
				new LogisticsTroubleManagement.Domain.Entities.DamageType("湿気による損傷", "MOISTURE", "雨漏り、湿気による品質劣化"),
				new LogisticsTroubleManagement.Domain.Entities.DamageType("温度による損傷", "TEMPERATURE", "高温・低温による品質劣化"),
				new LogisticsTroubleManagement.Domain.Entities.DamageType("汚損", "CONTAMINATION", "汚れ、異物混入など"),
				new LogisticsTroubleManagement.Domain.Entities.DamageType("盗難", "THEFT", "商品の盗難・紛失")
			};

			await _context.DamageTypes.AddRangeAsync(damageTypes);
			_logger.LogInformation("{Count}件の損傷タイプデータを追加しました。", damageTypes.Count);
		}

		// Warehousesの投入
		if (!await _context.Warehouses.AnyAsync())
		{
			var warehouses = new List<LogisticsTroubleManagement.Domain.Entities.Warehouse>
			{
				new LogisticsTroubleManagement.Domain.Entities.Warehouse("東京倉庫", "東京都江東区", "03-1234-5678", "tokyo@example.com"),
				new LogisticsTroubleManagement.Domain.Entities.Warehouse("大阪倉庫", "大阪府大阪市", "06-2345-6789", "osaka@example.com"),
				new LogisticsTroubleManagement.Domain.Entities.Warehouse("名古屋倉庫", "愛知県名古屋市", "052-3456-7890", "nagoya@example.com"),
				new LogisticsTroubleManagement.Domain.Entities.Warehouse("福岡倉庫", "福岡県福岡市", "092-4567-8901", "fukuoka@example.com")
			};

			await _context.Warehouses.AddRangeAsync(warehouses);
			_logger.LogInformation("{Count}件の倉庫データを追加しました。", warehouses.Count);
		}

		// ShippingCompaniesの投入
		if (!await _context.ShippingCompanies.AnyAsync())
		{
			var shippingCompanies = new List<LogisticsTroubleManagement.Domain.Entities.ShippingCompany>
			{
				new LogisticsTroubleManagement.Domain.Entities.ShippingCompany("ヤマト運輸", "YAMATO", "03-1111-2222", "yamato@example.com"),
				new LogisticsTroubleManagement.Domain.Entities.ShippingCompany("佐川急便", "SAGAWA", "03-3333-4444", "sagawa@example.com"),
				new LogisticsTroubleManagement.Domain.Entities.ShippingCompany("日本通運", "NITTSU", "03-5555-6666", "nittsu@example.com"),
				new LogisticsTroubleManagement.Domain.Entities.ShippingCompany("福山通運", "FUKUYAMA", "03-7777-8888", "fukuyama@example.com")
			};

			await _context.ShippingCompanies.AddRangeAsync(shippingCompanies);
			_logger.LogInformation("{Count}件の配送会社データを追加しました。", shippingCompanies.Count);
		}
	}
}
