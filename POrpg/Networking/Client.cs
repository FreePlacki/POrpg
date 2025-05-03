using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace POrpg.Networking;

public class Client
{
    private readonly IPEndPoint _endPoint;
    private TcpClient? _client;
    private NetworkStream _stream;
    
    public Client(string address = "127.0.0.1", int port = 5555)
    {
        _endPoint = new IPEndPoint(IPAddress.Parse(address), port);
    }

    public async Task Connect()
    {
        _client = new();
        await _client.ConnectAsync(_endPoint);
        _stream = _client.GetStream();
    }

    public async Task<string> Receive()
    {
        Debug.Assert(_client != null);
        Debug.Assert(_client.Connected);
        
        using var memoryStream = new MemoryStream();
        var buffer = new byte[8192];
        int totalBytesRead = 0;
        
        while (true)
        {
            int bytesRead = await _stream.ReadAsync(buffer);
            if (bytesRead == 0) break;
            
            await memoryStream.WriteAsync(buffer.AsMemory(0, bytesRead));
            totalBytesRead += bytesRead;
            
            if (!_stream.DataAvailable) break;
        }
        
        return Encoding.UTF8.GetString(memoryStream.ToArray());
    }
}