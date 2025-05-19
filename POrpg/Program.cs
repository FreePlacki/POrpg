using System.Net;
using System.Net.Sockets;
using POrpg.ConsoleUtils;
using POrpg.Controllers;
using POrpg.Networking;

namespace POrpg;

class Program
{
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

    private static void Usage()
    {
        Console.Error.WriteLine("Usage:");
        Console.Error.WriteLine($"\t{Environment.GetCommandLineArgs().First()} [--server [port]] [--client [ip:port]]");
        Environment.Exit(1);
    }

    private static void WriteError(string message) =>
        Console.Error.WriteLine(new StyledText(message, Style.Red));

    private record Options(bool IsServer, IPAddress Address, int Port);

    private static Options ParseOptions(string[] args)
    {
        if (args.Length > 2) Usage();

        var isServer = args.Length == 0
            ? ServerPrompt()
            : args[0] switch
            {
                "--server" => true,
                "--client" => false,
                _ => throw new ArgumentException($"Unknown argument: {args[0]}")
            };

        var address = IPAddress.Parse("127.0.0.1");
        var port = 5555;

        if (args.Length == 2)
        {
            var value = args[1];
            try
            {
                if (isServer)
                {
                    port = int.Parse(value);
                }
                else
                {
                    var parts = value.Split(':');
                    address = IPAddress.Parse(parts[0]);
                    port = int.Parse(parts[1]);
                }
            }
            catch
            {
                throw new ArgumentException(isServer
                    ? $"Invalid port number: {value}"
                    : $"Invalid address: {value}");
            }
        }

        return new Options(isServer, address, port);
    }

    private static async Task Main(string[] args)
    {
        Options opts;
        try
        {
            opts = ParseOptions(args);
        }
        catch (ArgumentException e)
        {
            WriteError(e.Message);
            Usage();
            return;
        }

        if (opts.IsServer)
        {
            try
            {
                _ = new ServerController(opts.Port);
            }
            catch (SocketException e)
            {
                WriteError($"Could not start the server ({e.Message}).");
                return;
            }
        }

        var client = new Client(opts.Address, opts.Port);
        try
        {
            await client.Connect();
        }
        catch (SocketException e)
        {
            WriteError($"Could not connect to the server ({e.Message}).");
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

        try
        {
            await clientController.MainLoop();
        }
        catch (IOException)
        {
            Console.Clear();
            WriteError("Lost connection to the server.");
            Console.CursorVisible = true;
            return;
        }

        Console.CursorVisible = true;
        Console.Clear();
    }
}