using System.Net.Sockets;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using POrpg.ConsoleHelpers;
using POrpg.Dungeon;
using POrpg.Effects;
using POrpg.Enemies;
using POrpg.Inventory;
using POrpg.Items;
using POrpg.Items.Modifiers;
using POrpg.Items.Modifiers.WeaponModifiers;
using POrpg.Items.Potions;
using POrpg.Items.Weapons;
using POrpg.Networking;

namespace POrpg;

class Program
{
    private const int RoomWidth = 41;
    private const int RoomHeight = 21;
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
        var director = new DungeonDirector();
        var serializerOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            IncludeFields = true,
            Converters =
            {
                new TwoDimensionalArrayJsonConverter<Tile>(), new PolymorphicConverterFactory<Tile>(),
                new PolymorphicConverterFactory<Item>(), new PolymorphicConverterFactory<Enemy>(),
                new PolymorphicConverterFactory<Effect>(), new PolymorphicConverterFactory<Potion>(),
                new PolymorphicConverterFactory<Weapon>(),
                new PolymorphicConverterFactory<Modifier>(), new PolymorphicConverterFactory<WeaponModifier>(),
            }
        };

        _isServer = ServerPrompt();
        if (_isServer)
        {
            using var server = new Server();
            server.Start();

            while (true)
            {
                var gc = InitializeGame(director, server, serializerOptions);
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

            var gc = await InitializeGame(director, client, serializerOptions);
            RunGame(gc);

            return;
        }

        Console.Clear();
        Console.CursorVisible = true;
    }

    static async Task<GameController> InitializeGame(DungeonDirector director, Client client,
        JsonSerializerOptions opts)
    {
        var msg = await client.Receive();

        var t = new FloorTile();
        var tj = JsonSerializer.Serialize<Tile>(t, opts);
        Console.WriteLine(tj);
        var tt = JsonSerializer.Deserialize<Tile>(tj, opts);

        // Environment.Exit(0);
        var dungeon = JsonSerializer.Deserialize<Dungeon.Dungeon>(msg, opts);
        var instructions = director.Build(new InstructionsBuilder());
        (int margin, int width)[] columns = [(0, RoomWidth), (2, 38), (2, 38)];
        ConsoleHelper.Initialize(instructions, columns, 3);

        return new GameController(dungeon!);
    }

    static GameController InitializeGame(DungeonDirector director, Server server, JsonSerializerOptions opts)
    {
        var dungeonBuilder =
            new DungeonBuilder(InitialDungeonState.Filled, RoomWidth, RoomHeight, PlayerInitialPosition);
        var dungeon = director.Build(dungeonBuilder);

        var instructions = director.Build(new InstructionsBuilder());
        (int margin, int width)[] columns = [(0, RoomWidth), (2, 38), (2, 38)];
        ConsoleHelper.Initialize(instructions, columns, 3);

        var gc = new GameController(dungeon);

        server.ClientConnected += async (_, id) =>
            await server.SendTo(id, JsonSerializer.SerializeToUtf8Bytes(dungeon, opts));

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