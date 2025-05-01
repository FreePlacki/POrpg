using System.Net.Sockets;
using System.Text.Json;
using System.Text.Json.Serialization;
using POrpg.ConsoleHelpers;
using POrpg.Dungeon;

namespace POrpg;

class Program
{
    private const int RoomWidth = 3;
    private const int RoomHeight = 3;
    private static readonly Position PlayerInitialPosition = (0, 0);

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

    private static bool _isServer;

    static async Task Main(string[] _)
    {
        _isServer = ServerPrompt();
        if (_isServer)
        {
            using var server = new Server();
            server.Start();

            var director = new DungeonDirector();
            var instructions = director.Build(new InstructionsBuilder());
            (int margin, int width)[] columns = [(0, RoomWidth), (2, 38), (2, 38)];
            ConsoleHelper.Initialize(instructions, columns, 3);
            
            while (true)
            {
                var gc = InitializeGame(director, server);
                var playAgain = RunGame(gc);
                TurnManager.GetInstance().Reset();
                if (!playAgain) break;
            }
        }
        else
        {
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

            var gc = await InitializeGame(client);
            RunGame(gc);
            
            return;
        }

        Console.Clear();
        Console.CursorVisible = true;
    }

    static async Task<GameController> InitializeGame(Client client)
    {
        var msg = await client.Receive();
        Console.WriteLine(msg);
        Console.WriteLine(msg);
        var dungeon = JsonSerializer.Deserialize<Dungeon.Dungeon>(msg);
        Environment.Exit(0);
        return new GameController(dungeon!);
    }
    
    static GameController InitializeGame(DungeonDirector director, Server server)
    {
        var dungeonBuilder =
            new DungeonBuilder(InitialDungeonState.Filled, RoomWidth, RoomHeight, PlayerInitialPosition);
        var dungeon = director.Build(dungeonBuilder);
        var opts = new JsonSerializerOptions
        {
            WriteIndented = true,
            ReferenceHandler = ReferenceHandler.Preserve,
            IncludeFields = true,
            // DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            Converters = { new TwoDimensionalIntArrayJsonConverter<Tile>() },
        };
        
        var gc = new GameController(dungeon);

        server.ClientConnected += async (_, id) => await server.SendTo(id, JsonSerializer.Serialize(dungeon, opts));
        
        return gc;
    }
    
    static bool RunGame(GameController gc)
    {
        Console.CursorVisible = false;
        Console.CancelKeyPress += (_, _) =>
        {
            Console.CursorVisible = true;
            Console.Clear();
        };
        Console.Clear();

        return gc.MainLoop();
    }
}