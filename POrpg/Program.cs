using POrpg.ConsoleHelpers;
using POrpg.Dungeon;

namespace POrpg;

class Program
{
    static void Main(string[] _)
    {
        const int roomWidth = 41;
        const int roomHeight = 21;
        Position playerInitialPosition = (0, 0);
        
        var dungeonBuilder =
            new DungeonBuilder(InitialDungeonState.Filled, roomWidth, roomHeight, playerInitialPosition);
        var director = new DungeonDirector();
        var dungeon = director.Build(dungeonBuilder);
        var instructions = director.Build(new InstructionsBuilder());
        var inputHandler = director.Build(new InputHandlerBuilder());

        Console.CursorVisible = false;
        Console.CancelKeyPress += (_, _) =>
        {
            Console.CursorVisible = true;
            Console.Clear();
        };
        Console.Clear();

        (int margin, int width)[] columns = [(0, roomWidth), (2, 38), (2, 38)];
        var console = ConsoleHelper.Initialize(instructions, columns, 3);

        while (true)
        {
            if (console.IsShowingInstructions)
            {
                if (Console.ReadKey(true).KeyChar == '?')
                    console.HideInstructions();
                else continue;
            }

            dungeon.Draw();
            console.Reset();

            var input = Console.ReadKey(true);

            dungeon.ProcessInput(inputHandler, input);
            if (dungeon.ShouldQuit)
                break;
        }
        Console.Clear();
    }
}