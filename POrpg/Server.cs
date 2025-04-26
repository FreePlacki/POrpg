using System.Collections.Concurrent;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace POrpg;

public class Server : IDisposable
{
    private const int MaxClients = 9;
    private readonly ConcurrentDictionary<int, TcpClient> _clients = new();
    private readonly TcpListener _listener;
    private readonly CancellationTokenSource _cts = new();

    public Server(int port = 5555)
    {
        _listener = new TcpListener(IPAddress.Any, port);
    }

    public void Start()
    {
        _listener.Start();
        _ = Task.Run(() => AcceptLoopAsync(_cts.Token));
    }

    private void Stop()
    {
        _cts.Cancel();
        _listener.Stop();

        foreach (var (_, client) in _clients)
            client.Close();

        _clients.Clear();
    }

    private async Task AcceptLoopAsync(CancellationToken token)
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
        catch (Exception ex)
        {
            Console.WriteLine($"Unexpected error in accept loop: {ex}");
        }
    }

    private async Task HandleClient(TcpClient client, CancellationToken token)
    {
        await using var stream = client.GetStream();

        var id = GetFreeClientId();
        if (id == -1)
        {
            const string msg = "Server full, try again later";
            _ = stream.WriteAsync(Encoding.UTF8.GetBytes(msg), token);
            client.Close();
            return;
        }

        var added = _clients.TryAdd(id, client);
        Debug.Assert(added);

        try
        {
            var welcome = $"Welcome player {id}";
            await stream.WriteAsync(Encoding.UTF8.GetBytes(welcome), token);
        }
        catch (Exception e)
        {
            Console.WriteLine($"Client error: {e.Message}");
        }
        finally
        {
            // TODO
            // lock (_clients) { _clients.Remove(client); }
            client.Close();
        }
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