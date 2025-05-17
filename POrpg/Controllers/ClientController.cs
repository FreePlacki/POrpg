using System.Text.Json;
using POrpg.ConsoleHelpers;
using POrpg.Dungeon;
using POrpg.InputHandlers;
using POrpg.Networking;

namespace POrpg.Controllers;

public class ClientController
{
    private ConsoleView _view;
    private readonly Client _client;
    private InputHandler _inputHandler;

    public ClientController(Client client)
    {
        _client = client;
    }

    public async Task Initialize()
    {
        var id = int.Parse(await _client.Receive());
        var dungeonMsg = await _client.Receive();
        var instructions = await _client.Receive();

        var dungeon = JsonSerializer.Deserialize<Dungeon.Dungeon>(dungeonMsg, ServerController.SerializerOptions)!;
        (int margin, int width)[] columns = [(0, dungeon.Width), (2, dungeon.Width - 3), (2, dungeon.Width - 3)];
        ConsoleHelper.Initialize(instructions, columns, 3);

        _view = new ConsoleView(dungeon, id);
        _inputHandler = new InputHandlerBuilder().Build(_view);

        _ = Task.Run(ViewUpdateLoop);
    }

    private async Task ViewUpdateLoop()
    {
        while (!_view.ShouldQuit)
        {
            var dungeonMsg = await _client.Receive();
            var dungeon = JsonSerializer.Deserialize<Dungeon.Dungeon>(dungeonMsg, ServerController.SerializerOptions)!;
            _view.Dungeon = dungeon;
            DisplayView();
        }
    }

    private void DisplayView()
    {
        _view.SetHints(_inputHandler.GetHints().ToArray());
        _view.Draw();
        ConsoleHelper.GetInstance().Reset();
    }

    public async Task<bool> MainLoop()
    {
        DisplayView();
        var console = ConsoleHelper.GetInstance();
        while (true)
        {
            if (console.IsShowingInstructions)
            {
                if (Console.ReadKey(true).KeyChar == '?')
                    console.HideInstructions();
                else continue;
            }

            _inputHandler = new InputHandlerBuilder().Build(_view);

            var input = Console.ReadKey(true);
            await ProcessInput(_inputHandler, input);

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

    private async Task ProcessInput(InputHandler handler, ConsoleKeyInfo input)
    {
        var command = handler.HandleInput(input);
        await _client.Send(JsonSerializer.Serialize(command, ServerController.SerializerOptions));
        command.Execute(_view);

        if (command.Description != null)
            ConsoleHelper.GetInstance().AddNotification(command.Description);
        if (command.AdvancesTurn)
            TurnManager.GetInstance().NextTurn();
    }
}