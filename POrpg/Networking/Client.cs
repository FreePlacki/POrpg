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

        return await Server.Receive(_stream);
    }

    public async Task Send(string msg)
    {
        Debug.Assert(_client != null);
        Debug.Assert(_client.Connected);

        await Server.Send(_stream, Encoding.UTF8.GetBytes(msg));
    }
}