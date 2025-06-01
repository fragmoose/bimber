using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Bimber
{
    public class ImageUploader
    {
        private readonly global::AppSettings settings;
        private readonly HttpClient httpClient;

        public ImageUploader(global::AppSettings settings, HttpClient httpClient = null)
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
                var imageContent = new StreamContent(imageStream);
                imageContent.Headers.ContentType = MediaTypeHeaderValue.Parse("image/jpeg"); // Możesz dostosować typ MIME
                content.Add(imageContent, "source", fileName);

                var request = new HttpRequestMessage(HttpMethod.Post, "https://pixvid.org/api/1/upload");
                request.Headers.Add("X-API-Key", settings.ApiKey);
                request.Content = content;

                var response = await httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync();
                var responseObject = JsonConvert.DeserializeObject<ApiResponse>(responseContent);

                return responseObject?.Image?.Url ?? throw new InvalidOperationException("Nie można uzyskać URL obrazu z odpowiedzi API");
            }
        }

        private class ApiResponse
        {
            [JsonProperty("status_code")]
            public int StatusCode { get; set; }

            [JsonProperty("success")]
            public SuccessInfo Success { get; set; }

            [JsonProperty("image")]
            public ImageInfo Image { get; set; }

            [JsonProperty("status_txt")]
            public string StatusText { get; set; }
        }

        private class SuccessInfo
        {
            [JsonProperty("message")]
            public string Message { get; set; }

            [JsonProperty("code")]
            public int Code { get; set; }
        }

        private class ImageInfo
        {
            [JsonProperty("url")]
            public string Url { get; set; }

            // Możesz dodać inne właściwości z odpowiedzi, jeśli będą potrzebne
        }
    }
}
