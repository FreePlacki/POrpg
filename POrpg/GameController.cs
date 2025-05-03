using POrpg.ConsoleHelpers;
using POrpg.Dungeon;
using POrpg.InputHandlers;

namespace POrpg;

public class GameController
{
    private readonly Dungeon.Dungeon _dungeon;
    private readonly ConsoleView _view;
    public GameController(Dungeon.Dungeon dungeon, int playerId)
    {
        _dungeon = dungeon;
        _view = new ConsoleView(_dungeon, playerId);
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
        var command = handler.HandleInput(_view, _dungeon, input);
        command.Execute();
        if (command.Description != null)
            ConsoleHelper.GetInstance().AddNotification(command.Description);
        if (command.AdvancesTurn)
            TurnManager.GetInstance().NextTurn();
    }
}