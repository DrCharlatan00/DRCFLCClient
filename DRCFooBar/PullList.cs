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
                #error Not Work Dot use
                try
                {
                    if (!File.Exists(name))
                        return (false, $"File not found: {name}");

                    if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    {
                        // Windows: foobar2000
                        var startInfo = new ProcessStartInfo
                        {
                            FileName = @"C:\Program Files\foobar2000\foobar2000.exe",
                            UseShellExecute = true
                        };
                        // foobar принимает файлы просто как аргументы
                        startInfo.ArgumentList.Add(name);

                        Process.Start(startInfo);
                        Console.WriteLine($"[Windows] Pushed to foobar2000: {name}");
                    }
                    else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                    {
                        // Linux: VLC


                        var startInfo = new ProcessStartInfo
                        {
                            FileName = "vlc",
                            UseShellExecute = false
                        };

                        // Только ArgumentList - никаких Arguments!
                        startInfo.ArgumentList.Add("--playlist-enqueue");
                        startInfo.ArgumentList.Add("--no-video-title-show");
                        startInfo.ArgumentList.Add(name); // Путь к файлу добавится корректно, даже с пробелами

                        using var process = Process.Start(startInfo);
                        Console.WriteLine($"[Linux] Enqueued to VLC: {name}");
                    }

                    
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Error starting player: {ex.Message}");
                    Console.ForegroundColor = ConsoleColor.White;
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
