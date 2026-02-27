//#define LoadFromCode
//    #define NoUseUnsafeCode

using System.Runtime.InteropServices;
using System.Text;
using FLCDownloaderAudio;

namespace DRCFLCClient;

internal class Program
{

    public struct ConnectionSetting
    {
        public string Ip_Serv;
        public string Port;
        public ConnectionSetting()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                EnvReader.Load(".env");
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                try
                {
                    if (!Directory.Exists(@"/etc/drflcclient"))
                    {
                        Directory.CreateDirectory(@"/etc/drflcclient");
                        string ContentBase = @"IP_SERV=0.0.0.0
PORT=5000";
                        File.WriteAllText(@"/etc/drflcclient/.env",ContentBase);
                    }
                    EnvReader.Load(@"/etc/drflcclient/.env");
                }
                catch (UnauthorizedAccessException ex )
                {
                    Console.WriteLine(@"Please open in sudo mode or create Directory  '/etc/drflcclient' and create File '/etc/drflcclient'");
                    //throw;
                    Environment.Exit(-150);
                }
               
            }

            
#if LoadFromCode
    Ip_Serv = "localhost";
    Port = "5213";
    return;
#endif
            Ip_Serv = Environment.GetEnvironmentVariable("IP_SERV") ?? "localhost";
            Port = Environment.GetEnvironmentVariable("PORT") ?? "5213";
            if (string.IsNullOrWhiteSpace(Ip_Serv))
            {
                throw new Exception($"{nameof(Ip_Serv)} is null");
            }
            if (string.IsNullOrWhiteSpace(Port))
            {
                throw new Exception($"{nameof(Port)} is null");
            }
        
        }

        
    }

    
    public static List<string>? ListAudio;
    public static PullList PullAudioList;
    

    private static async Task Main(string[] args)
    {
       
        Console.InputEncoding = Encoding.UTF8;
        Console.OutputEncoding = Encoding.UTF8;
        //Task.Run(()=> {});
        PullAudioList = new();
        
        //signature of events
        PullAudioList.EventPullingTrackError += () => {  
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine("Error on pulling ");
            
        };
        ConnectionSetting connection = new();

#if DEBUG
        Console.WriteLine("Try Connect to Server FLAC");
#endif


        if (args is not null)
        {
            foreach (string val in args)
            {
                if (val == "list")
                {
                    var clientList = new FLCDowloader($"http://{connection.Ip_Serv}:{connection.Port}", false);
                    ListAudio = await clientList.GetMusicListAsync();
                    if (ListAudio.Count == 0)
                    {
                        Console.WriteLine("List is null. Exit ");
                        Environment.Exit(-12);
                    }
                    int count = 0;
                    foreach (var Audio in ListAudio)
                    {
                        count++;
                        Console.WriteLine($"№ {count}: {Audio}");
                    }
                }
                
            }
        }
        try {
          
            var clientTest = new FLCDowloader($"http://{connection.Ip_Serv}:{connection.Port}", false);
        }
        catch (Exceptions.NoAvaibleConnectToServer)
        {
            Console.WriteLine("No Available to connect Flac server");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Sorry unexpected error {ex.Message}");
            await Task.Delay(3000);
            Environment.Exit(-1);
        }

        var client = new FLCDowloader($"http://{connection.Ip_Serv}:{connection.Port}", true);
        client.EventServerDead += () =>
        {
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine("Connect to server died");
            Environment.Exit(-500);
        };
        client.NullListSounds += () =>
        {
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine("On Server List is null. Please restart server ");
            Environment.Exit(-501);
        };
        try
        {
            ListAudio = await client.GetMusicListAsync();
        }
        catch (Exceptions.NullListAvaibleFlacMusic exs)
        {
            Console.WriteLine(exs.Message);
        }
        int counter = 0;
        List<string> AudioNames = new();
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("Accessible Music: ");
        Console.ForegroundColor = ConsoleColor.White;
        foreach (var Audio in ListAudio)
        {
            counter++;
            AudioNames.Add(Audio);
            Console.WriteLine($"{counter}: {Audio}");

        }
        
        while (true)
        {
            Console.Write("Write Audio or Command: ");
            var AudioName = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(AudioName))
            {
                Console.WriteLine("Text is null, please write again");
                continue;
            }
            else if (AudioName.ToLower() == "exit" || AudioName.ToLower() == "e") {
                Environment.Exit(0);
            }
            else if (AudioName.ToLower() == "i") {
                string audio = NavigateListAudio.NavigateList(AudioNames);
                foreach (var Audio in AudioNames)
                {
                    if (audio == Audio) {
                        await client.DownloadFlcAsync(audio, $"{audio.ToLower()}");
                        PullAudioList.AddToList(audio);
                    }
                }

                continue;
            }
            /* else if (AudioName.ToLower() == "push" || AudioName.ToLower() == "p") {
            PullAudioList.SendToPull(false);
            continue;
        }*/
            else if (AudioName.ToLower() == "lpush" || AudioName.ToLower() == "lp")
            {
                try
                {
                    await PullAudioList.SendToPull(true);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error on local push ");
                    throw;
                }
                
                continue;
            }
            else if (AudioName.ToLower() == "rm")
            {
                var answ = Comander.CommanderExec(EnumParamtr.REMOVE_FILES);
                switch (answ.Answer)
                {
                    case Comander.EnumAnswersReturnCommander.Skip:
                        Console.WriteLine(answ.Messange);
                        break;
                    case Comander.EnumAnswersReturnCommander.Error:
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                        Console.WriteLine(answ.Messange);
                        Console.ForegroundColor = ConsoleColor.White;
                        break;
                }
                continue;
            }
            else if (AudioName == "list")
            {
                Console.Clear();
                foreach (var item in PullAudioList.PullNames)
                {
                    Console.WriteLine(item);
                }

                continue;
            }
            bool status = true;
            if (AudioName.Contains(".flac")) {
                try
                {
                    await client.DownloadFlcAsync(AudioName, $"{AppDomain.CurrentDomain.BaseDirectory} \\ {AudioName.ToLower() ?? "Skip"}");
                }
                catch {
                    Console.WriteLine("No find or Error on FLCServer");
                    status = false;
                }
            }
            else { //review this
                try
                {
                    await client.DownloadFlcAsync(AudioName += ".flac", $" {AppDomain.CurrentDomain.BaseDirectory} \\ {AudioName.ToLower() ?? "Skip"}.flac");
                }
                catch
                {
                    status = false;
                    Console.WriteLine("No find or Error on FLCServer");
                }
            }
            if (status) {

                PullAudioList.AddToList(AudioName); 
                Console.BackgroundColor = ConsoleColor.Green;
                Console.WriteLine("Successful Push ");
                _ = Task.Delay(500);
                Console.BackgroundColor = ConsoleColor.Black;
            }
                
           
        }

       
    }
    
    
}