using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Bimber
{
    public class ImageUploader2 : IImageUploader
    {
        private readonly global::AppSettings settings;
        private readonly HttpClient httpClient;

        public ImageUploader2(global::AppSettings settings, HttpClient httpClient = null)
        {
            this.settings = settings ?? throw new ArgumentNullException(nameof(settings));
            this.httpClient = httpClient ?? new HttpClient();
        }

        public async Task<string> UploadImageAsync(Stream imageStream, string fileName)
        {
            if (imageStream == null)
                throw new ArgumentNullException(nameof(imageStream));
            if (string.IsNullOrWhiteSpace(fileName))
                throw new ArgumentNullException(nameof(fileName));

            using (var content = new MultipartFormDataContent())
            {
                // Add image
                var imageContent = new StreamContent(imageStream);
                imageContent.Headers.ContentType = MediaTypeHeaderValue.Parse("image/jpeg");
                content.Add(imageContent, "file", fileName);

                var request = new HttpRequestMessage(HttpMethod.Post, "https://fmapi.net/api/v2/image");
                request.Headers.Authorization = new AuthenticationHeaderValue(settings.ApiKey);
                request.Content = content;

                var response = await httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync();
                var responseObject = JsonConvert.DeserializeObject<ApiResponse>(responseContent);

                return responseObject?.Data?.Url ?? throw new InvalidOperationException("Could not get image URL from API response");
            }
        }

        private class ApiResponse
        {
            [JsonProperty("data")]
            public ImageData? Data { get; set; }

            [JsonProperty("status")]
            public required string Status { get; set; }
        }

        private class ImageData
        {
            [JsonProperty("id")]
            public required string Id { get; set; }

            [JsonProperty("url")]
            public required string Url { get; set; }
        }
    }
}