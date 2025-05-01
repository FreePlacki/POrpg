using POrpg.ConsoleHelpers;
using POrpg.Dungeon;

namespace POrpg;

public class GameController
{
    private readonly Dungeon.Dungeon _dungeon;
    public GameController(Dungeon.Dungeon dungeon)
    {
        _dungeon = dungeon;
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

            var inputHandler = new InputHandlerBuilder().Build(_dungeon);
            
            var cv = new ConsoleView(_dungeon, inputHandler.GetHints().ToArray());
            cv.Draw();

            console.Reset();

            var input = Console.ReadKey(true);
            _dungeon.ProcessInput(inputHandler, input);
        
            if (_dungeon.Player.Attributes[Attribute.Health] <= 0)
            {
                console.ShowDeathScreen();
                Console.ReadKey(true);
                return true;
            }
        
            if (_dungeon.ShouldQuit)
                return false;
        }
    }
}