using POrpg.ConsoleHelpers;
using POrpg.Dungeon;

namespace POrpg;

class Program
{
    static void Main(string[] _)
    {
        const int roomWidth = 40;
        const int roomHeight = 20;
        var dungeon = new ConcreteDungeonBuilder(InitialDungeonState.Filled, roomWidth, roomHeight)
            .AddCentralRoom()
            .AddRandomChambers(4)
            .Build();

        Console.CursorVisible = false;
        Console.Clear();

        (int start, int width)[] columns = [(0, roomWidth), (roomWidth + 5, 80)];
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