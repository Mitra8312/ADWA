
namespace ADWA.Services
{
	public class FileCleaner : BackgroundService
	{
		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			while (!stoppingToken.IsCancellationRequested)
			{
				_ = await ClearUploads();

				await Task.Delay(10000 * 60 * 24, stoppingToken);
			}
		}

		private async Task<bool> ClearUploads()
		{
			try
			{
				string path = Directory.GetCurrentDirectory();

				path = Path.Combine(path, "wwwroot", "uploads");

				List<string> uploads = Directory.GetFiles(path).ToList();

				int deletedFilesCount = checked(uploads.Count - 3);

				for (int i = 0; i < deletedFilesCount; i++)
				{
					File.Delete(uploads[i]);
				}

				return true;
			}
			catch
			{
				return false;
			}
		}
	}
}
