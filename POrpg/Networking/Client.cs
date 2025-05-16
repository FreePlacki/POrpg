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

        var lenBuffer = new byte[4];
        await _stream.ReadExactlyAsync(lenBuffer, 0, 4);
        var msgLen = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(lenBuffer, 0));

        using var memoryStream = new MemoryStream(msgLen);
        var buffer = new byte[8192];
        var remaining = msgLen;
        while (remaining > 0)
        {
            var toRead = Math.Min(remaining, buffer.Length);
            await _stream.ReadExactlyAsync(buffer, 0, toRead);

            await memoryStream.WriteAsync(buffer.AsMemory(0, toRead));
            remaining -= toRead;
        }

        return Encoding.UTF8.GetString(memoryStream.ToArray());
    }
}