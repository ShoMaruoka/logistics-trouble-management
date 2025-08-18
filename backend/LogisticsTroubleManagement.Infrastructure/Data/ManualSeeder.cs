using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;

namespace LogisticsTroubleManagement.Infrastructure.Data;

public static class ManualSeeder
{
	public static async Task SeedAsync()
	{
		// 設定の読み込み
		var configuration = new ConfigurationBuilder()
			.SetBasePath(Path.GetDirectoryName(typeof(ManualSeeder).Assembly.Location) ?? Directory.GetCurrentDirectory())
			.AddJsonFile("appsettings.json", optional: false)
			.Build();

		// Serilogの設定
		Log.Logger = new LoggerConfiguration()
			.ReadFrom.Configuration(configuration)
			.CreateLogger();

		// サービスコレクションの設定
		var services = new ServiceCollection();

		// DbContextの登録
		services.AddDbContext<ApplicationDbContext>(options =>
			options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

		// DatabaseSeederの登録
		services.AddScoped<DatabaseSeeder>();

		// ログの設定
		services.AddLogging(builder =>
		{
			builder.AddSerilog();
		});

		using var serviceProvider = services.BuildServiceProvider();

		try
		{
			Log.Information("サンプルデータ投入を開始します...");

			// DatabaseSeederの取得と実行
			var seeder = serviceProvider.GetRequiredService<DatabaseSeeder>();
			await seeder.SeedAsync();

			Log.Information("サンプルデータ投入が完了しました。");
		}
		catch (Exception ex)
		{
			Log.Error(ex, "サンプルデータ投入中にエラーが発生しました。");
			throw;
		}
		finally
		{
			Log.CloseAndFlush();
		}
	}
}
