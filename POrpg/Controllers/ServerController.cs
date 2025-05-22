using POrpg.ConsoleUtils;
using POrpg.Dungeon;
using POrpg.Networking;

namespace POrpg.Controllers;

public class ServerController
{
    private readonly Dungeon.Dungeon _dungeon;
    private readonly string _instructions;
    private readonly Server _server;
    private readonly Queue<int> _turns = [];
    private readonly Lock _lock = new();

    public ServerController(int port)
    {
        var director = new DungeonDirector();
        var dungeonBuilder =
            new DungeonBuilder(InitialDungeonState.Filled, 41, 21);
        _dungeon = director.Build(dungeonBuilder);
        _instructions = director.Build(new InstructionsBuilder());

        _server = new Server(port);
        _server.Start();
        _server.ClientConnected += OnClientConnected;
        _server.ClientDisconnected += OnClientDisconnected;
        _server.MessageReceived += OnMessageReceived;
    }

    // on using async void: https://learn.microsoft.com/en-us/archive/msdn-magazine/2013/march/async-await-best-practices-in-asynchronous-programming#avoid-async-void
    private async void OnClientConnected(object? _, int id)
    {
        lock (_lock)
        {
            _dungeon.AddPlayer(id);
            _turns.Enqueue(id);
        }

        await _server.SendTo(id, new JoinMessage(id, _dungeon, _instructions));
        await _server.SendToAll(
            new NotificationMessage($"Player {new StyledText(id.ToString(), Styles.Player)} connected"), except: [id]);
        await _server.SendToAll(new StateMessage(_dungeon), except: [id]);
    }

    private async void OnClientDisconnected(object? _, int id)
    {
        lock (_lock)
        {
            _dungeon.RemovePlayer(id);
        }

        await _server.SendToAll(new StateMessage(_dungeon));
    }

    private async void OnMessageReceived(object? _, (int playerId, IMessage msg) data)
    {
        var command = (data.msg as CommandMessage)!.Command;
        var currentTurn = _turns.First();
        if (!_dungeon.Players.ContainsKey(currentTurn))
        {
            _turns.Dequeue();
            currentTurn = _turns.First();
        }

        if (command.AdvancesTurn && currentTurn != data.playerId)
        {
            await _server.SendTo(data.playerId,
                new NotificationMessage(
                    $"It's Player {new StyledText(currentTurn.ToString(), Styles.Player)}'s turn."));
            return;
        }

        lock (_lock)
        {
            command.Execute(_dungeon, data.playerId);

            if (command.AdvancesTurn)
            {
                _turns.Dequeue();
                _turns.Enqueue(data.playerId);
                _dungeon.TurnManager.CurrentlyPlaying = _turns.First();
                _dungeon.NextTurn();
            }
        }

        if (_dungeon.Players[data.playerId].Attributes[Attribute.Health] <= 0)
        {
            await _server.SendToAll(
                new NotificationMessage($"Player {new StyledText(data.playerId.ToString(), Styles.Player)} died"),
                except: [data.playerId]);
            await _server.SendTo(data.playerId, new NotificationMessage("You died!"));
            await _server.SendTo(data.playerId, new YouDiedMessage());
            _dungeon.RemovePlayer(data.playerId);
        }

        if (command.Description != null)
            await _server.SendTo(data.playerId, new NotificationMessage(command.Description));

        await _server.SendToAll(new StateMessage(_dungeon));
    }
}