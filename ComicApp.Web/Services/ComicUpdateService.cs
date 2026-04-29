using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.IO.Compression;

namespace ComicApp.Web.Services
{
    public class ComicUpdateService : BackgroundService
    {
        private readonly ILogger<ComicUpdateService> _logger;
        private readonly IWebHostEnvironment _env;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly TimeSpan _checkInterval = TimeSpan.FromHours(24);

        private const string DatasetUrl =
            "https://www.bl.uk/bibliographic/downloads/ComicsResearcherFormat_202204_csv.zip";

        public ComicUpdateService(
            ILogger<ComicUpdateService> logger,
            IWebHostEnvironment env,
            IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _env = env;
            _httpClientFactory = httpClientFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("ComicUpdateService started.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await CheckForUpdatesAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "ComicUpdateService encountered an error during update check.");
                }

                await Task.Delay(_checkInterval, stoppingToken);
            }
        }

        private async Task CheckForUpdatesAsync()
        {
            _logger.LogInformation("Checking for dataset updates from British Library...");

            var dataPath = Path.Combine(_env.WebRootPath, "data");
            var zipPath = Path.Combine(dataPath, "comics_update.zip");

            var client = _httpClientFactory.CreateClient();

            // Download the zip file
            var response = await client.GetAsync(DatasetUrl);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Failed to download dataset update. Status: {Status}", response.StatusCode);
                return;
            }

            var newBytes = await response.Content.ReadAsByteArrayAsync();

            // Compare with existing zip if present to avoid unnecessary extraction
            if (File.Exists(zipPath))
            {
                var existingBytes = await File.ReadAllBytesAsync(zipPath);
                if (newBytes.SequenceEqual(existingBytes))
                {
                    _logger.LogInformation("Dataset is already up to date. No changes detected.");
                    return;
                }
            }

            // Save the new zip
            await File.WriteAllBytesAsync(zipPath, newBytes);
            _logger.LogInformation("New dataset downloaded. Extracting CSV files...");

            // Extract CSV files into wwwroot/data
            using var zipStream = new System.IO.MemoryStream(newBytes);
            using var archive = new System.IO.Compression.ZipArchive(zipStream, System.IO.Compression.ZipArchiveMode.Read);

            foreach (var entry in archive.Entries)
            {
                if (entry.Name.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
                {
                    var destPath = Path.Combine(dataPath, entry.Name);
                    entry.ExtractToFile(destPath, overwrite: true);
                    _logger.LogInformation("Extracted: {File}", entry.Name);
                }
            }

            _logger.LogInformation("Dataset update complete. Existing user data unaffected.");
        }
    }
}