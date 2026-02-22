//#define LoadFromCode
//    #define NoUseUnsafeCode
using DRCFLCClient;
using FLCDownloaderAudio;
using System.Text;

internal class Program
{

    public struct ConnectionSetting
    {
        public string Ip_Serv ;
        public string Port;
        public ConnectionSetting()
        {
            EnvReader.Load(".env");
#if LoadFromCode
    Ip_Serv = "localhost";
    Port = "5213";
    return;
#endif
        Ip_Serv = Environment.GetEnvironmentVariable("IP_SERV");
        Port = Environment.GetEnvironmentVariable("PORT");
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
                    if (ListAudio is not null)
                    {
                        int count = 0;
                        foreach (var Audio in ListAudio)
                        {
                            count++;
                            Console.WriteLine($"№ {count}: {Audio}");
                        }
                    }
                }
                
            }
        }
        try {
          
            var clientTest = new FLCDowloader($"http://{connection.Ip_Serv}:{connection.Port}", false);
        }
        catch (Exceptions.NoAvaibleConnectToServer)
        {
            Console.WriteLine("No Avaible to connect Flac server");
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
            Environment.Exit(500);
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
            Console.Write("Write Audio: ");
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
                string audio = NavigateListAudio.NavigateList<string>(AudioNames);
                foreach (var Audio in AudioNames)
                {
                    if (audio == Audio) {
                        await client.DownloadFlcAsync(audio, $"{audio.ToLower()}");
                        PullAudioList.AddToList(audio);
                    }
                }

                continue;
            }
            else if (AudioName.ToLower() == "push" || AudioName.ToLower() == "p") {
                PullAudioList.SendToPull(false);
                continue;
            }
            else if (AudioName.ToLower() == "lpush" || AudioName.ToLower() == "lp")
            {
                PullAudioList.SendToPull(true);
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
                    await client.DownloadFlcAsync(AudioName, $"{AppDomain.CurrentDomain.BaseDirectory} \\ {AudioName.ToLower()}");
                }
                catch {
                    Console.WriteLine("No find or Error on FLCServer");
                    status = false;
                }
            }
            else {
                try
                {
                    await client.DownloadFlcAsync(AudioName += ".flac", $" {AppDomain.CurrentDomain.BaseDirectory} \\ {AudioName.ToLower()}.flac");
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
                Console.WriteLine("PASS");
                Console.BackgroundColor = ConsoleColor.Black;
            }
                
           
        }

       
    }
    
    
}
