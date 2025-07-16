using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Threading.Tasks;

namespace Bimber
{
    public class UpdateInstaller
    {
        private readonly string _githubRepoUrl;
        private readonly string _tempDownloadPath;
        private readonly string _appDirectory;

        public UpdateInstaller(string githubRepoUrl)
        {
            _githubRepoUrl = githubRepoUrl;
            _tempDownloadPath = Path.Combine(Path.GetTempPath(), "BimberUpdate");
            _appDirectory = AppDomain.CurrentDomain.BaseDirectory;
        }

        public async Task DownloadAndInstallUpdateAsync()
        {
            try
            {
                var downloadUrl = await GetLatestReleaseDownloadUrlAsync();
                var downloadedFile = await DownloadUpdateAsync(downloadUrl);
                await InstallUpdateAsync(downloadedFile);
            }
            catch (Exception ex)
            {
                throw new Exception(Resources.UpdateFailed + ": " + ex.Message);
            }
        }

        private async Task<string> GetLatestReleaseDownloadUrlAsync()
        {
            var apiUrl = _githubRepoUrl.Replace("github.com", "api.github.com/repos") + "/releases/latest";

            using (var client = new WebClient())
            {
                client.Headers.Add("User-Agent", "BimberUpdater");
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

        private async Task InstallUpdateAsync(string updatePackagePath)
        {
          
            var extractPath = Path.Combine(_tempDownloadPath, "extracted");
            ZipFile.ExtractToDirectory(updatePackagePath, extractPath, true);

           
            var batchFilePath = Path.Combine(_tempDownloadPath, "update.bat");
            await CreateUpdateBatchFile(batchFilePath, extractPath);

            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = batchFilePath,
                    WindowStyle = ProcessWindowStyle.Hidden,
                    CreateNoWindow = true
                }
            };

            process.Start();

           
            Environment.Exit(0);
        }

        private async Task CreateUpdateBatchFile(string batchFilePath, string updateFilesPath)
        {
            var lines = new[]
            {
                "@echo off",
                "echo Updating Bimber...",
                "timeout /t 2 /nobreak >nul",
                "",
                "xcopy /y /e \"" + updateFilesPath + "\\*\" \"" + _appDirectory + "\"",
                "",
                "echo Running new version...",
                "start \"\" \"" + Path.Combine(_appDirectory, "Bimber.exe") + "\"",
                "",
                "echo Removing temp files...",
                "rmdir /s /q \"" + _tempDownloadPath + "\"",
                "exit"
            };

            await File.WriteAllLinesAsync(batchFilePath, lines);
        }
    }
}