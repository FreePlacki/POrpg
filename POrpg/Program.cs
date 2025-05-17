using System.Net.Sockets;
using POrpg.Controllers;
using POrpg.Networking;

namespace POrpg;

class Program
{
    private static ServerController? _serverController;

    private static bool ServerPrompt()
    {
        Console.WriteLine("Start as (S)erver or (C)lient");
        while (true)
        {
            var input = Console.ReadKey(true);
            switch (input.Key)
            {
                case ConsoleKey.S:
                    return true;
                case ConsoleKey.C:
                    return false;
            }
        }
    }

    private static async Task Main(string[] _)
    {
        var isServer = ServerPrompt();
        if (isServer) _serverController = new ServerController();

        var client = new Client();
        try
        {
            await client.Connect();
        }
        catch (SocketException)
        {
            Console.WriteLine("Could not connect to the server.");
            return;
        }

        TurnManager.GetInstance().Reset();
        var clientController = new ClientController(client);
        await clientController.Initialize();

        Console.CursorVisible = false;
        Console.CancelKeyPress += (_, _) =>
        {
            Console.CursorVisible = true;
            Console.Clear();
        };
        Console.Clear();

        clientController.MainLoop();
    }
}