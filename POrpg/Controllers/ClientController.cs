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
    }

    public async Task<bool> MainLoop()
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
            await ProcessInput(inputHandler, input);

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
        var dungeonMsg = await _client.Receive();
        var dungeon = JsonSerializer.Deserialize<Dungeon.Dungeon>(dungeonMsg, ServerController.SerializerOptions)!;
        _view.Dungeon = dungeon;

        command.Execute(_view);

        if (command.Description != null)
            ConsoleHelper.GetInstance().AddNotification(command.Description);
        if (command.AdvancesTurn)
            TurnManager.GetInstance().NextTurn();
    }
}