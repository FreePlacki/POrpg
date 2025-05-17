using System.Collections.Concurrent;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace POrpg.Networking;

public class Server : IDisposable
{
    private const int MaxClients = 10;
    private readonly ConcurrentDictionary<int, NetworkStream> _clients = new();
    private readonly TcpListener _listener;
    private readonly CancellationTokenSource _cts = new();

    public event EventHandler<int>? ClientConnected;
    public event EventHandler<(int, string)>? MessageReceived;

    public Server(int port = 5555)
    {
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
            await stream.WriteAsync("Server full, try again later"u8.ToArray(), token);
            client.Close();
            return;
        }

        var added = _clients.TryAdd(id, stream);
        Debug.Assert(added);

        ClientConnected?.Invoke(this, id);
        _ = Task.Run(() => ReceiveLoop(id, token), token);
    }

    private async Task ReceiveLoop(int clientId, CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            var client = _clients[clientId];
            var msg = await Receive(client);
            MessageReceived?.Invoke(this, (clientId, msg));
        }
    }

    public async Task SendToAll(byte[] msg)
    {
        foreach (var id in _clients.Keys)
            await SendTo(id, msg);
    }

    public async Task SendToAll(int id, string msg) => await SendToAll(Encoding.UTF8.GetBytes(msg));

    public async Task SendTo(int id, string msg) => await SendTo(id, Encoding.UTF8.GetBytes(msg));

    public async Task SendTo(int id, byte[] msg) => await Send(_clients[id], msg);

    public static async Task<string> Receive(NetworkStream stream)
    {
        var lenBuffer = new byte[4];
        await stream.ReadExactlyAsync(lenBuffer, 0, 4);
        var msgLen = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(lenBuffer, 0));

        using var memoryStream = new MemoryStream(msgLen);
        var buffer = new byte[8192];
        var remaining = msgLen;
        while (remaining > 0)
        {
            var toRead = Math.Min(remaining, buffer.Length);
            await stream.ReadExactlyAsync(buffer, 0, toRead);

            await memoryStream.WriteAsync(buffer.AsMemory(0, toRead));
            remaining -= toRead;
        }

        return Encoding.UTF8.GetString(memoryStream.ToArray());
    }

    public static async Task Send(NetworkStream stream, byte[] msg)
    {
        await stream.WriteAsync(BitConverter.GetBytes(IPAddress.HostToNetworkOrder(msg.Length)));
        await stream.WriteAsync(msg);
    }

    public static async Task Send(NetworkStream stream, string msg) => await Send(stream, Encoding.UTF8.GetBytes(msg));

    private int GetFreeClientId()
    {
        var occupied = new bool[MaxClients];
        foreach (var (id, _) in _clients)
            occupied[id] = true;

        return occupied.ToList().IndexOf(false);
    }

    public void Dispose() => Stop();
}