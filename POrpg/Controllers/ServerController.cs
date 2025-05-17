using POrpg.ConsoleUtils;
using POrpg.Dungeon;
using POrpg.Networking;

namespace POrpg.Controllers;

public class ServerController
{
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

    // on using async void: https://learn.microsoft.com/en-us/archive/msdn-magazine/2013/march/async-await-best-practices-in-asynchronous-programming#avoid-async-void
    private async void OnClientConnected(object? _, int id)
    {
        _dungeon.AddPlayer(id);
        await _server.SendTo(id, new JoinMessage(id, _dungeon, _instructions));
        await _server.SendToAll(
            new NotificationMessage($"Player {new StyledText(id.ToString(), Styles.Player)} connected"), except: [id]);
    }

    private async void OnMessageReceived(object? _, (int playerId, IMessage msg) data)
    {
        var command = (data.msg as CommandMessage)!.Command;
        command.Execute(_dungeon, data.playerId);

        if (command.AdvancesTurn)
            _dungeon.TurnManager.NextTurn();

        await _server.SendToAll(new StateMessage(_dungeon));
    }
}