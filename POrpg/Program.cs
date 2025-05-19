using System.Net.Sockets;
using POrpg.ConsoleUtils;
using POrpg.Controllers;
using POrpg.Networking;

namespace POrpg;

class Program
{
    private static ServerController? _serverController;

    private static bool ServerPrompt()
    {
        Console.WriteLine(
            $"Start as {new StyledText("S", Styles.Player)}erver or {new StyledText("C", Styles.Player)}lient?");
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
        if (isServer)
        {
            try
            {
                _serverController = new ServerController();
            }
            catch (SocketException e)
            {
                Console.WriteLine(new StyledText($"Could not start the server ({e.Message}).", Style.Red));
                return;
            }
        }

        var client = new Client();
        try
        {
            await client.Connect();
        }
        catch (SocketException e)
        {
            Console.WriteLine(new StyledText($"Could not connect to the server ({e.Message}).", Style.Red));
            return;
        }

        var clientController = new ClientController(client);
        await clientController.Initialize();

        Console.CursorVisible = false;
        Console.CancelKeyPress += (_, _) =>
        {
            Console.CursorVisible = true;
            Console.Clear();
        };
        Console.Clear();

        await clientController.MainLoop();

        Console.CursorVisible = true;
        Console.Clear();
    }
}