using System.Net.Http;
using System.Net.Http.Json;
using System.Runtime.InteropServices.Marshalling;
using static System.Net.WebRequestMethods;

namespace FLCDownloaderAudio
{
    public class FLCDowloader
    {
        public readonly HttpClient httpClient;
        public FLCDowloader(string HttpURL, bool SkipCheckConnection) {
            httpClient = new HttpClient
            {
                BaseAddress = new Uri(HttpURL)
            };
            if (SkipCheckConnection) return;
            try
            {
                var dt = httpClient.GetStringAsync("/alive");
                if (dt is null) {
                    throw new Exceptions.NoAvaibleConnectToServer();
                }
            }
            catch
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Err to ping FLCServer");
                Console.ForegroundColor = ConsoleColor.White;
                Task.Delay(500);
                Environment.Exit(-2);
            }
        }

        public async Task DownloadFlcAsync(string name, string FilePath) {
            try
            {
                await httpClient.GetAsync("/alive");
            }
            catch
            {
                throw new Exceptions.NoAvaibleConnectToServer("Err whis connect to Flac Server");
            }
            
            var response = await httpClient.GetAsync($"audio/{name}", HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();

            await using var stream = await response.Content.ReadAsStreamAsync();
            await using var file = System.IO.File.Create(FilePath);

            await stream.CopyToAsync(file);
        }

        public async Task<List<string>> GetMusicListAsync() {
            var list = await httpClient.GetFromJsonAsync<List<string>>("/list");
            if (list is null)
            {
                throw new Exceptions.NullListAvaibleFlacMusic("Null list Music, Check Music folder!");
            }
            return list;
        }


    }


}
