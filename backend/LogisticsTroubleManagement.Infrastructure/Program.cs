using LogisticsTroubleManagement.Infrastructure.Data;

namespace LogisticsTroubleManagement.Infrastructure;

public class Program
{
	public static async Task Main(string[] args)
	{
		try
		{
			Console.WriteLine("物流トラブル管理システム - サンプルデータ投入ツール");
			Console.WriteLine("================================================");
			
			await ManualSeeder.SeedAsync();
			
			Console.WriteLine("サンプルデータ投入が正常に完了しました。");
		}
		catch (Exception ex)
		{
			Console.WriteLine($"エラーが発生しました: {ex.Message}");
			Console.WriteLine($"詳細: {ex}");
		}
		
		Console.WriteLine("Enterキーを押して終了してください...");
		Console.ReadLine();
	}
}
