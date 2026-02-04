using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Text.Json;

public class FoobarClient
{
    private ClientWebSocket _ws;
    private CancellationTokenSource _cts;

    public event Action<TrackInfo> TrackChanged;

    public async Task StartAsync()
    {
        _ws = new ClientWebSocket();
        _cts = new CancellationTokenSource();

        await _ws.ConnectAsync(new Uri("ws://localhost:8880/api/player/subscribe"), CancellationToken.None);

        _ = Task.Run(ReceiveLoop); // слушаем события
    }

    private async Task ReceiveLoop()
    {
        var buffer = new byte[8192];

        while (_ws.State == WebSocketState.Open)
        {
            var result = await _ws.ReceiveAsync(buffer, _cts.Token);
            var json = Encoding.UTF8.GetString(buffer, 0, result.Count);

            try
            {
                using var doc = JsonDocument.Parse(json);

                if (doc.RootElement.TryGetProperty("player", out var player))
                {
                    if (player.TryGetProperty("now_playing", out var nowPlaying))
                    {
                        var track = new TrackInfo
                        {
                            Title = nowPlaying.GetProperty("title").GetString(),
                            Artist = nowPlaying.GetProperty("artist").GetString()
                        };

                        TrackChanged?.Invoke(track);
                    }
                }
            }
            catch
            {
                // игнорируем мусорные пакеты
            }
        }
    }
}

public class TrackInfo
{
    public string Title { get; set; }
    public string Artist { get; set; }
}
