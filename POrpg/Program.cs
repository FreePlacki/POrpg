using System.Net.Sockets;
using System.Text.Json;
using POrpg.ConsoleHelpers;
using POrpg.Dungeon;
using POrpg.Effects;
using POrpg.Enemies;
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
        using var server = new Server();
        if (_isServer)
        {
            InitializeGame(director, server, serializerOptions);
            server.Start();
            Console.Clear();
        }

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
        TurnManager.GetInstance().Reset();
        RunGame(gc);
    }

    static async Task<GameController> InitializeGame(DungeonDirector director, Client client,
        JsonSerializerOptions opts)
    {
        var id = int.Parse(await client.Receive());
        var msg = await client.Receive();

        var dungeon = JsonSerializer.Deserialize<Dungeon.Dungeon>(msg, opts)!;
        var instructions = director.Build(new InstructionsBuilder());
        (int margin, int width)[] columns = [(0, RoomWidth), (2, 38), (2, 38)];
        ConsoleHelper.Initialize(instructions, columns, 3);

        dungeon.AddPlayer(id);
        return new GameController(dungeon, id);
    }

    static void InitializeGame(DungeonDirector director, Server server, JsonSerializerOptions opts)
    {
        var dungeonBuilder =
            new DungeonBuilder(InitialDungeonState.Filled, RoomWidth, RoomHeight, PlayerInitialPosition);
        var dungeon = director.Build(dungeonBuilder);

        server.ClientConnected += async (_, id) =>
        {
            await server.SendTo(id, id.ToString());
            await server.SendTo(id, JsonSerializer.SerializeToUtf8Bytes(dungeon, opts));
        };
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