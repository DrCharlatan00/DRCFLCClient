//#define LoadFromCode
using FLCDownloaderAudio;

internal class Program
{

    public struct ConnectionSetting
    {
        public string Ip_Serv ;
        public string Port;
        public ConnectionSetting()
        {

#if LoadFromCode
    Ip_Serv = "192.168.1.50";
    Port = "5000";
    return;
#endif
    Ip_Serv = Environment.GetEnvironmentVariable("IP_SERV");
    Port = Environment.GetEnvironmentVariable("PORT");
    if(Ip_Serv == string.IsNullOrWhiteSpace())
            {
                throw new Exception($"{nameof(Ip_Serv)} is null");
            }
    if(Port == string.IsNullOrWhiteSpace())
            {
                throw new Exception($"{nameof(Port)} is null");
            }
        
        }

        
    }

    
    public static List<string> ListAudio;
    
    private static async Task Main(string[] args)
    {
        ConnectionSetting connection = new();

#if DEBUG
Console.WriteLine("Try Connect to Server FLAC");
#endif


        if( args is not null )
        { 
            if(args == "list")
            {
                var client = new FLCDowloader($"http://{connection.Ip_Serv}:{connection.Port}");
                ListAudio =  await client.GetMusicListAsync();
                if(ListAudio is not null)
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
        try{
        var client = new FLCDowloader("http://192.168.1.50:5000");
        }
        catch  (Exceptions.NoAvaibleConnectToServer)
        {
            Console.WriteLine("No Avaible to connect Flac server");
        }
        catch(Exception ex)
        {
            Console.WriteLine("Sorry unexpected error");
            Task.Delay(3000);
            Environment.Exit(-1);
        }

        try
        {
        ListAudio = await client.GetMusicListAsync();    
        }
        catch(Exceptions.NullListAvaibleFlacMusic exs)
        {
            Console.WriteLine(exs.message);
        }
        

        //await client.DownloadFlcAsync("Alorn On Danse.flac", "resp.flac");
    }
}