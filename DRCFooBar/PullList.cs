using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;


namespace DRCFLCClient
{
    public class PullList
    {
        public List<string> PullNames = new List<string>();

        public async void SendToPull(bool Local) {
            foreach (var item in PullNames)
            {
                await AddTrackAsync(item);
            }
            PullNames.Clear();
            
        }

        public void AddToList(string AudioName){
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Add to Pull list {AudioName}");
            Console.ForegroundColor = ConsoleColor.White;
            PullNames.Add(AudioName);

        }

        private async Task<(bool Ok, string? err)> AddTrackAsync(string name)
        {
            var args = string.Join(" ", PullNames.Select(f => $"\"{f}\""));
            
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) { //start whis foobar Windows
                Console.WriteLine("Pushing");
                Process.Start(new ProcessStartInfo { FileName = @"C:\Program Files\foobar2000\foobar2000.exe", Arguments = args, UseShellExecute = true });
                Console.WriteLine("End Push");
            } 
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) { Console.WriteLine("Linux no avaible"); }


            return (true, null);
        }

    }
}
