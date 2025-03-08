using POrpg.ConsoleHelpers;

namespace POrpg;

class Program
{
    static void Main(string[] _)
    {
        const int roomWidth = 40;
        const int roomHeight = 20;
        var room = new Room(roomWidth, roomHeight);

        Console.CursorVisible = false;
        Console.Clear();
        
        (int start, int width)[] columns = [(0, roomWidth), (roomWidth + 5, 80)];
        var console = new ConsoleHelper(columns);
        
        while (true)
        {
            room.Draw(console);
            console.Reset();
            
            var input = Console.ReadKey(true);
            if (input.Key == ConsoleKey.C) Console.Clear();
            room.ProcessInput(input);
        }
        // ReSharper disable once FunctionNeverReturns
    }
}