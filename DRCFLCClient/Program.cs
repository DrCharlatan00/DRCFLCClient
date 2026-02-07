#define LoadFromCode
//    #define NoUseUnsafeCode
using DRCFLCClient;
using FLCDownloaderAudio;

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
        Task.Run(()=> {});
        PullAudioList = new();
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
            #warning Replace ip
            var clientTest = new FLCDowloader("http://192.168.1.50:5000", false);
        }
        catch (Exceptions.NoAvaibleConnectToServer)
        {
            Console.WriteLine("No Avaible to connect Flac server");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Sorry unexpected error");
            Task.Delay(3000);
            Environment.Exit(-1);
        }

        var client = new FLCDowloader($"http://{connection.Ip_Serv}:{connection.Port}", true);

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
                Console.WriteLine("No pls again");
            }
            else if (AudioName == "exit" || AudioName == "e") {
                Environment.Exit(0);
            }
            else if (AudioName == "i"){
                string audio = NavigateListAudio.NavigateList<string>(AudioNames);
                PullAudioList.AddToList(audio);
                continue;
            }
            bool status = true;
            if (AudioName.Contains(".flac")) {
                try
                {
                    await client.DownloadFlcAsync(AudioName, $"{AudioName.ToLower()}");
                }
                catch {
                    Console.WriteLine("No find or Error on FLCServer");
                    status = false;
                }
            }
            else {
                try
                {
                    await client.DownloadFlcAsync(AudioName += ".flac", $"{AudioName.ToLower()}.flac");
                }
                catch
                {
                    status = false;
                    Console.WriteLine("No find or Error on FLCServer");
                }
            }
            if (status) {
//#error Check This pls)
                PullAudioList.AddToList(AudioName);
                Console.BackgroundColor = ConsoleColor.Green;
                Console.WriteLine("PASS");
                Console.BackgroundColor = ConsoleColor.Black;
            }
                
           
        }
    }
}