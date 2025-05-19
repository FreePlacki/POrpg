using System.Diagnostics;
using System.Net;
using System.Net.Sockets;

namespace POrpg.Networking;

public class Client
{
    private readonly IPEndPoint _endPoint;
    private TcpClient? _client;
    private NetworkStream? _stream;

    public Client(IPAddress address, int port)
    {
        _endPoint = new IPEndPoint(address, port);
    }

    public async Task Connect()
    {
        _client = new();
        await _client.ConnectAsync(_endPoint);
        _stream = _client.GetStream();
    }

    public async Task<IMessage> Receive()
    {
        Debug.Assert(_client != null);
        Debug.Assert(_client.Connected);

        return await Server.Receive(_stream!);
    }

    public async Task Send(IMessage message)
    {
        Debug.Assert(_client != null);
        Debug.Assert(_client.Connected);

        await Server.Send(_stream!, message);
    }
}