using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace Bimber
{
    public class UpdateInstaller
    {
        private readonly string _githubRepoUrl;
        private readonly string _tempDownloadPath;

        public UpdateInstaller(string githubRepoUrl)
        {
            _githubRepoUrl = githubRepoUrl;
            _tempDownloadPath = Path.Combine(Path.GetTempPath(), "MyAppUpdate");
        }

        public async Task DownloadAndInstallUpdateAsync()
        {
            try
            {
                // Get download URL from GitHub
                var downloadUrl = await GetLatestReleaseDownloadUrlAsync();

                // Download the update
                var downloadedFile = await DownloadUpdateAsync(downloadUrl);

                // Install the update
                InstallUpdate(downloadedFile);
            }
            catch (Exception ex)
            {
                // Handle errors
                throw new Exception("Update failed: " + ex.Message);
            }
        }

        private async Task<string> GetLatestReleaseDownloadUrlAsync()
        {
            var apiUrl = _githubRepoUrl.Replace("github.com", "api.github.com/repos") + "/releases/latest";

            using (var client = new WebClient())
            {
                client.Headers.Add("User-Agent", "UpdateInstaller");
                var response = await client.DownloadStringTaskAsync(apiUrl);

                dynamic releaseInfo = Newtonsoft.Json.JsonConvert.DeserializeObject(response);
                return releaseInfo.assets[0].browser_download_url;
            }
        }

        private async Task<string> DownloadUpdateAsync(string downloadUrl)
        {
            if (!Directory.Exists(_tempDownloadPath))
            {
                Directory.CreateDirectory(_tempDownloadPath);
            }

            var fileName = Path.GetFileName(new Uri(downloadUrl).LocalPath);
            var filePath = Path.Combine(_tempDownloadPath, fileName);

            using (var client = new WebClient())
            {
                await client.DownloadFileTaskAsync(downloadUrl, filePath);
            }

            return filePath;
        }

        private void InstallUpdate(string installerPath)
        {
            // For MSI or EXE installers
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = installerPath,
                    UseShellExecute = true
                }
            };

            process.Start();

            // Close the current application
            Environment.Exit(0);
        }
    }
}
