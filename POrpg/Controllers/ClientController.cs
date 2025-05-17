using System.Text.Json;
using POrpg.ConsoleHelpers;
using POrpg.Dungeon;
using POrpg.InputHandlers;
using POrpg.Networking;

namespace POrpg.Controllers;

public class ClientController
{
    private Dungeon.Dungeon _dungeon;
    private ConsoleView _view;
    private readonly Client _client;

    public ClientController(Client client)
    {
        _client = client;
    }

    public async Task Initialize()
    {
        var id = int.Parse(await _client.Receive());
        var dungeonMsg = await _client.Receive();
        var instructions = await _client.Receive();

        _dungeon = JsonSerializer.Deserialize<Dungeon.Dungeon>(dungeonMsg, ServerController.SerializerOptions)!;
        (int margin, int width)[] columns = [(0, _dungeon.Width), (2, _dungeon.Width - 3), (2, _dungeon.Width - 3)];
        ConsoleHelper.Initialize(instructions, columns, 3);

        _view = new ConsoleView(_dungeon, id);
    }

    public bool MainLoop()
    {
        var console = ConsoleHelper.GetInstance();
        while (true)
        {
            if (console.IsShowingInstructions)
            {
                if (Console.ReadKey(true).KeyChar == '?')
                    console.HideInstructions();
                else continue;
            }

            var inputHandler = new InputHandlerBuilder().Build(_view);
            _view.SetHints(inputHandler.GetHints().ToArray());

            _view.Draw();

            console.Reset();

            var input = Console.ReadKey(true);
            ProcessInput(inputHandler, input);

            if (_view.Player.Attributes[Attribute.Health] <= 0)
            {
                console.ShowDeathScreen();
                Console.ReadKey(true);
                return true;
            }

            if (_view.ShouldQuit)
                return false;
        }
    }

    private void ProcessInput(InputHandler handler, ConsoleKeyInfo input)
    {
        var command = handler.HandleInput(input);
        command.Execute(_dungeon, _view.PlayerId);
        command.Execute(_view);
        if (command.Description != null)
            ConsoleHelper.GetInstance().AddNotification(command.Description);
        if (command.AdvancesTurn)
            TurnManager.GetInstance().NextTurn();
    }
}