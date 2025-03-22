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

        Console.CursorVisible = false;
        Console.CancelKeyPress += (_, _) => Console.CursorVisible = true;
        Console.Clear();

        (int margin, int width)[] columns = [(0, roomWidth), (5, 38), (5, 38)];
        var console = ConsoleHelper.Initialize(columns);

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
            if (input.Key == ConsoleKey.C)
                Console.Clear();
            if (input.KeyChar == '?')
            {
                console.ShowInstructions(instructions);
                continue;
            }

            dungeon.ProcessInput(input);
        }
        // ReSharper disable once FunctionNeverReturns
    }
}