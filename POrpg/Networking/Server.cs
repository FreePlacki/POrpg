using System.Collections.Concurrent;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace POrpg.Networking;

public class Server : IDisposable
{
    private const int MaxClients = 10;
    private readonly ConcurrentDictionary<int, NetworkStream> _clients = new();
    private readonly TcpListener _listener;
    private readonly CancellationTokenSource _cts = new();

    private readonly Dungeon.Dungeon _dungeon;
    private readonly JsonSerializerOptions _serializerOptions;

    public event EventHandler<int>? ClientConnected;

    public Server(int port = 5555)
    {
        // _dungeon = dungeon;
        // _serializerOptions = options;
        _listener = new TcpListener(IPAddress.Any, port);
    }

    public void Start()
    {
        _listener.Start();
        _ = Task.Run(() => AcceptLoop(_cts.Token));
    }

    private void Stop()
    {
        _cts.Cancel();
        _listener.Stop();

        foreach (var (_, client) in _clients)
            client.Close();

        _clients.Clear();
    }

    private async Task AcceptLoop(CancellationToken token)
    {
        try
        {
            while (!token.IsCancellationRequested)
            {
                var client = await _listener.AcceptTcpClientAsync(token);
                await HandleClient(client, token);
            }
        }
        catch (OperationCanceledException)
        {
            // expected on Stop()
        }
    }

    private async Task HandleClient(TcpClient client, CancellationToken token)
    {
        var stream = client.GetStream();

        var id = GetFreeClientId();
        if (id == -1)
        {
            const string msg = "Server full, try again later";
            await stream.WriteAsync(Encoding.UTF8.GetBytes(msg), token);
            client.Close();
            return;
        }

        var added = _clients.TryAdd(id, stream);
        Debug.Assert(added);
        // TODO
        // var joinMsg = new JoinAckMessage(id, JsonSerializer.SerializeToUtf8Bytes(_dungeon, _serializerOptions));
        // await SendTo(id, joinMsg);

        ClientConnected?.Invoke(this, id);
    }

    public async Task SendToAll(string msg)
    {
        foreach (var id in _clients.Keys)
            await SendTo(id, msg);
    }

    public async Task SendTo(int id, string msg)
    {
        await SendTo(id, Encoding.UTF8.GetBytes(msg));
    }

    public async Task SendTo(int id, byte[] msg)
    {
        await _clients[id].WriteAsync(BitConverter.GetBytes(IPAddress.HostToNetworkOrder(msg.Length)));
        await _clients[id].WriteAsync(msg);
    }

    private int GetFreeClientId()
    {
        var occupied = new bool[MaxClients];
        foreach (var (id, _) in _clients)
            occupied[id] = true;

        return occupied.ToList().IndexOf(false);
    }

    public void Dispose() => Stop();
}