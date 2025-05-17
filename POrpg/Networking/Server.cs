using System.Collections.Concurrent;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using POrpg.Commands;
using POrpg.Effects;
using POrpg.Enemies;
using POrpg.Items;
using POrpg.Items.Modifiers;
using POrpg.Items.Modifiers.WeaponModifiers;
using POrpg.Items.Potions;
using POrpg.Items.Weapons;

namespace POrpg.Networking;

public class Server : IDisposable
{
    private const int MaxClients = 10;
    private readonly ConcurrentDictionary<int, NetworkStream> _clients = new();
    private readonly TcpListener _listener;
    private readonly CancellationTokenSource _cts = new();

    public event EventHandler<int>? ClientConnected;
    public event EventHandler<(int, IMessage)>? MessageReceived;

    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        WriteIndented = true,
        IncludeFields = true,
        IgnoreReadOnlyFields = false,
        IgnoreReadOnlyProperties = false,
        Converters =
        {
            new TwoDimensionalArrayJsonConverter<Tile>(),
            new PolymorphicConverterFactory<Tile>(),
            new PolymorphicConverterFactory<Item>(),
            new PolymorphicConverterFactory<Enemy>(),
            new PolymorphicConverterFactory<Effect>(),
            new PolymorphicConverterFactory<Potion>(),
            new PolymorphicConverterFactory<Weapon>(),
            new PolymorphicConverterFactory<Modifier>(),
            new PolymorphicConverterFactory<WeaponModifier>(),
            new PolymorphicConverterFactory<ICommand>(),
            new PolymorphicConverterFactory<IMessage>(),
        }
    };


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

    public async Task SendToAll(IMessage message, HashSet<int>? except = null)
    {
        foreach (var id in _clients.Keys.Where(id => except == null || !except.Contains(id)))
            await SendTo(id, message);
    }

    public async Task SendTo(int id, IMessage message) => await Send(_clients[id], message);

    public static async Task<IMessage> Receive(NetworkStream stream)
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

        var msg = Encoding.UTF8.GetString(memoryStream.ToArray());
        return JsonSerializer.Deserialize<IMessage>(msg, SerializerOptions)!;
    }

    public static async Task Send(NetworkStream stream, IMessage message)
    {
        var json = JsonSerializer.Serialize(message, SerializerOptions);
        var msg = Encoding.UTF8.GetBytes(json);
        await stream.WriteAsync(BitConverter.GetBytes(IPAddress.HostToNetworkOrder(msg.Length)));
        await stream.WriteAsync(msg);
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