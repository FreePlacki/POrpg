using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace POrpg;

public class Server
{
    private const int MaxClients = 10;
    private List<TcpClient> clients = new();
    
    public Server()
    {
    }
    
    public async Task Run()
    {
        var ipEndPoint = new IPEndPoint(IPAddress.Any, 6969);
        TcpListener listener = new(ipEndPoint);

        try
        {
            listener.Start();

            if (clients.Count == MaxClients)
            {
                // TODO wait
            }
            TcpClient handler = await listener.AcceptTcpClientAsync();
            await using NetworkStream stream = handler.GetStream();

            var message = $"📅 {DateTime.Now} 🕛";
            var dateTimeBytes = Encoding.UTF8.GetBytes(message);
            await stream.WriteAsync(dateTimeBytes);

            Console.WriteLine($"Sent message: \"{message}\"");
            
            var buffer = new byte[1_024];
            int received = await stream.ReadAsync(buffer);

            var msg = Encoding.UTF8.GetString(buffer, 0, received);
            // var person = JsonSerializer.Deserialize<Person>(msg);
            //
            // Console.WriteLine($"Person: {person.Name} of age {person.Age}");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
        finally
        {
            listener.Stop();
        }
    }
}