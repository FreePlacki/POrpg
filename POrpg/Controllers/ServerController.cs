using System.Text.Json;
using POrpg.Commands;
using POrpg.Dungeon;
using POrpg.Effects;
using POrpg.Enemies;
using POrpg.Items;
using POrpg.Items.Modifiers;
using POrpg.Items.Modifiers.WeaponModifiers;
using POrpg.Items.Potions;
using POrpg.Items.Weapons;
using POrpg.Networking;

namespace POrpg.Controllers;

public class ServerController
{
    public static readonly JsonSerializerOptions SerializerOptions = new()
    {
        WriteIndented = true,
        IncludeFields = true,
        IgnoreReadOnlyFields = false,
        IgnoreReadOnlyProperties = false,
        Converters =
        {
            new TwoDimensionalArrayJsonConverter<Tile>(),
            new PolymorphicConverterFactory<Tile>(),
            new PolymorphicConverterFactory<Item>(),
            new PolymorphicConverterFactory<Enemy>(),
            new PolymorphicConverterFactory<Effect>(),
            new PolymorphicConverterFactory<Potion>(),
            new PolymorphicConverterFactory<Weapon>(),
            new PolymorphicConverterFactory<Modifier>(),
            new PolymorphicConverterFactory<WeaponModifier>(),
            new PolymorphicConverterFactory<ICommand>(),
        }
    };

    private readonly Dungeon.Dungeon _dungeon;
    private readonly string _instructions;
    private readonly Server _server;

    public ServerController()
    {
        var director = new DungeonDirector();
        var dungeonBuilder =
            new DungeonBuilder(InitialDungeonState.Filled, 41, 21);
        _dungeon = director.Build(dungeonBuilder);
        _instructions = director.Build(new InstructionsBuilder());

        _server = new Server();
        _server.Start();
        _server.ClientConnected += OnClientConnected;
        _server.MessageReceived += OnMessageReceived;
    }

    // https://learn.microsoft.com/en-us/archive/msdn-magazine/2013/march/async-await-best-practices-in-asynchronous-programming#avoid-async-void
    private async void OnClientConnected(object? _, int id)
    {
        _dungeon.AddPlayer(id);
        await _server.SendTo(id, id.ToString());
        await _server.SendTo(id, JsonSerializer.SerializeToUtf8Bytes(_dungeon, SerializerOptions));
        await _server.SendTo(id, _instructions);
    }

    private async void OnMessageReceived(object? _, (int playerId, string msg) data)
    {
        var command = JsonSerializer.Deserialize<ICommand>(data.msg, SerializerOptions)!;
        command.Execute(_dungeon, data.playerId);

        await _server.SendToAll(JsonSerializer.SerializeToUtf8Bytes(_dungeon, SerializerOptions));
    }
}