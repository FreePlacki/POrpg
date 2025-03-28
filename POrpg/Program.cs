using POrpg.ConsoleHelpers;
using POrpg.Dungeon;
using POrpg.InputHandlers;

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

        (int margin, int width)[] columns = [(0, roomWidth), (2, 38), (2, 38)];
        var console = ConsoleHelper.Initialize(columns);
        var inputHandler =
            new MovementInputHandler(new CycleItemsHandler(new InventoryInputHandler(new GuardInputHandler())));

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

            dungeon.ProcessInput(inputHandler, input);
            if (dungeon.ShouldQuit)
                break;
        }
    }
}