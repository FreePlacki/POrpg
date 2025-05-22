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

    public void Disconnect()
    {
        _client?.Close();
    }

    public async Task<IMessage?> Receive()
    {
        if (_client?.Connected == false) return null;

        return await Server.Receive(_stream!);
    }

    public async Task Send(IMessage message)
    {
        if (_client?.Connected == false) return;

        await Server.Send(_stream!, message);
    }
}