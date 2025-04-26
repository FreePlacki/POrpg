using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace POrpg;

public class Client
{
    private IPEndPoint _endPoint;
    private TcpClient? _client;
    
    public Client(string address = "127.0.0.1", int port = 5555)
    {
        _endPoint = new IPEndPoint(IPAddress.Parse(address), port);
    }

    public async Task Connect()
    {
        _client = new();
        await _client.ConnectAsync(_endPoint);
    }

    public async Task<string> Receive()
    {
        Debug.Assert(_client != null);
        
        await using var stream = _client.GetStream();
        
        var buffer = new byte[1_024];
        int received = await stream.ReadAsync(buffer);

        var message = Encoding.UTF8.GetString(buffer, 0, received);
        return message;
    }
}