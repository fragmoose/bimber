using System;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;

namespace Bimber
{
    public class UpdateChecker
    {
        private readonly string _githubRepoUrl;
        private readonly string _currentVersion;

        public UpdateChecker(string githubRepoUrl)
        {
            _githubRepoUrl = githubRepoUrl;
            _currentVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }

        public async Task<bool> CheckForUpdatesAsync()
        {
            try
            {
                // Get the latest release version from GitHub
                var latestVersion = await GetLatestVersionAsync();

                // Compare with current version
                return new Version(latestVersion) > new Version(_currentVersion);
            }
            catch
            {
                return false;
            }
        }

        private async Task<string> GetLatestVersionAsync()
        {
            // GitHub API URL for releases
            var apiUrl = _githubRepoUrl.Replace("github.com", "api.github.com/repos") + "/releases/latest";

            using (var client = new WebClient())
            {
                // GitHub API requires a user agent
                client.Headers.Add("User-Agent", "UpdateChecker");

                var response = await client.DownloadStringTaskAsync(apiUrl);

                // Parse JSON to get tag_name (version)
                dynamic releaseInfo = Newtonsoft.Json.JsonConvert.DeserializeObject(response);
                return releaseInfo.tag_name;
            }
        }
    }
}
