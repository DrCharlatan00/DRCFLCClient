
using FLCDownloaderAudio;

internal class Program
{
    public static List<string> ListAudio;
    private static async Task Main(string[] args)
    {

        
        var client = new FLCDowloader("http://192.168.1.50:5000");

        ListAudio = await client.GetMusicListAsync();

        await client.DownloadFlcAsync("Alorn On Danse.flac", "resp.flac");
    }
}