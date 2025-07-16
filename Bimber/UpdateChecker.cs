using System;
using System.Diagnostics;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Bimber
{
    public class UpdateChecker
    {
        private readonly string _githubRepoUrl;
        private readonly string _currentVersion;

        public UpdateChecker(string githubRepoUrl)
        {
            _githubRepoUrl = githubRepoUrl;
            
            _currentVersion = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion;
        }

        public async Task<bool> CheckForUpdatesAsync()
        {
            try
            {
                string latestVersion = await GetLatestVersionAsync();

                if (Version.TryParse(_currentVersion, out Version current) &&
                    Version.TryParse(latestVersion, out Version latest))
                {
                    return latest > current; 
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        private async Task<string> GetLatestVersionAsync()
        {
            string apiUrl = _githubRepoUrl.Replace("github.com", "api.github.com/repos") + "/releases/latest";

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.UserAgent.ParseAdd("UpdateChecker");
                string response = await client.GetStringAsync(apiUrl);

                
                JObject releaseInfo = JObject.Parse(response);
                string tagName = releaseInfo["tag_name"].ToString();
                return tagName.StartsWith("v") ? tagName.Substring(1) : tagName; 
            }
        }
    }
}