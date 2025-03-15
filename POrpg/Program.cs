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
        var dungeon =
            new ConcreteDungeonBuilder(InitialDungeonState.Filled, roomWidth, roomHeight, playerInitialPosition)
                .AddCentralRoom()
                // .AddRandomChambers(2)
                .AddRandomPaths()
                .AddMoney()
                .AddUnusableItems(maxEffects: 3)
                .AddWeapons(maxEffects: 4)
                .Build();

        Console.CursorVisible = false;
        Console.CancelKeyPress += (_, _) => Console.CursorVisible = true;
        Console.Clear();

        (int start, int width)[] columns = [(0, roomWidth), (roomWidth + 5, 100)];
        var console = new ConsoleHelper(columns);

        while (true)
        {
            dungeon.Draw(console);
            console.Reset();

            var input = Console.ReadKey(true);
            if (input.Key == ConsoleKey.C) Console.Clear();
            dungeon.ProcessInput(input);
        }
        // ReSharper disable once FunctionNeverReturns
    }
}