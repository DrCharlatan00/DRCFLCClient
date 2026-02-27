using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;


namespace DRCFLCClient
{
    public class PullList
    {
        public delegate void TrackActions();
        public event TrackActions EventPullingTrackError;
        
        
        public List<string> PullNames = new();

        public async Task SendToPull(bool Local) {
            foreach (var item in PullNames)
            {
                var answ = await AddTrackAsync(item);
                if (!answ.Ok)
                {
                    Console.WriteLine(answ.err);
                }
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

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                //start whis foobar Windows
                Console.WriteLine("Pushing");
                Process.Start(new ProcessStartInfo
                {
                    FileName = @"C:\Program Files\foobar2000\foobar2000.exe", Arguments = args, UseShellExecute = true
                });
                Console.WriteLine("End Push");
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                try
                {
                    if (!File.Exists(name))
                        return (false, $"File not found: {name}");
                    
                        // Linux: Audacious
                        var PushInfo = new ProcessStartInfo
                        {
                            FileName = "audacious",
                            UseShellExecute = false
                        };

                        PushInfo.ArgumentList.Add("--enqueue");
                        PushInfo.ArgumentList.Add(name);

                        using var PushProcess = Process.Start(PushInfo);
                        return (true,null);
                    
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Error starting player: {ex.Message}");
                    Console.ForegroundColor = ConsoleColor.White;
                    EventPullingTrackError.Invoke();
                    return (false, ex.Message);
                }


                // xdg-open linux 
                /*else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    // Откроет файл в плеере ПО УМОЛЧАНИЮ в системе пользователя
                    var startInfo = new ProcessStartInfo
                    {
                        FileName = "xdg-open",
                        Arguments = $"\"{name}\"",
                        UseShellExecute = false
                    };
                    Process.Start(startInfo);
                }*/

                
            }
            return (true, null);
        }

    }
}
