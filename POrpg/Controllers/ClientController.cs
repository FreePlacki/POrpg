using POrpg.ConsoleUtils;
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
        var (id, dungeon, instructions) = (await _client.Receive() as JoinMessage)!;

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
            switch (await _client.Receive())
            {
                case StateMessage { Dungeon: var dungeon }:
                    _view.Dungeon = dungeon;
                    break;
                case NotificationMessage { Notification: var notification }:
                    ConsoleHelper.GetInstance().AddNotification(notification);
                    break;
            }

            _inputHandler = new InputHandlerBuilder().Build(_view);
            if (!ConsoleHelper.GetInstance().IsShowingInstructions)
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
                {
                    console.HideInstructions();
                    DisplayView();
                }
                else continue;
            }

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
        await _client.Send(new CommandMessage(command));
        command.Execute(_view);
        _inputHandler = new InputHandlerBuilder().Build(_view);
    }
}