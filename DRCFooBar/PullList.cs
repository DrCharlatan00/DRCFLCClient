using System.Text;
using System.Text.Json;


namespace DRCFLCClient
{
    public class PullList
    {
        public List<string> PullNames = new List<string>();

        public async void SendToPull(string AudioName) {
            PullNames.Add(AudioName);
            await AddTrackAsync(AudioName);
        }

        public void AddToList(string AudioName){
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Add to Pull list {AudioName}");
            Console.ForegroundColor = ConsoleColor.White;
            PullNames.Add(AudioName);

        }

        private async Task<(bool Ok, string? err)> AddTrackAsync(string name) {
            if (!System.IO.File.Exists(name)) {
                return (false, "No file");
            }
            var fl = Directory.GetCurrentDirectory() + name;
            using (HttpClient client = new()) {
                var body = new { items = new[] { "file:///" +  fl.Replace("\\", "/") } };
                var json = JsonSerializer.Serialize(body); 
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                await client.PostAsync("http://localhost:8880/api/player/enqueue", content);
            }
            return (true, null);

        }
    }
}
