using System.Net.Http;
using System.Net.Http.Json;
using System.Runtime.InteropServices.Marshalling;
using static System.Net.WebRequestMethods;

namespace FLCDownloaderAudio
{
    public class FLCDowloader
    {
        public readonly HttpClient httpClient;
        public FLCDowloader(string HttpURL) {
            httpClient = new HttpClient
            {
                BaseAddress = new Uri(HttpURL)
            };
        }

        public async Task DownloadFlcAsync(string name, string FilePath) {
            var response = await httpClient.GetAsync($"audio/{name}", HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();

            await using var stream = await response.Content.ReadAsStreamAsync();
            await using var file = System.IO.File.Create(FilePath);

            await stream.CopyToAsync(file);
        }

        public async Task<List<string>> GetMusicListAsync() {
            var list = await httpClient.GetFromJsonAsync<List<string>>("/list");
            return list ?? new List<string>();
        }


    }


}
