using POrpg.ConsoleHelpers;
using POrpg.Dungeon;

namespace POrpg;

class Program
{
    private const int RoomWidth = 41;
    private const int RoomHeight = 21;
    private static readonly Position PlayerInitialPosition = (0, 0);
    
    static void Main(string[] _)
    {
        var director = new DungeonDirector();
        var instructions = director.Build(new InstructionsBuilder());
        (int margin, int width)[] columns = [(0, RoomWidth), (2, 38), (2, 38)];
        ConsoleHelper.Initialize(instructions, columns, 3);
        
        bool playAgain = true;
    
        while (playAgain)
        {
            playAgain = RunGame(director);
            TurnManager.GetInstance().Reset();
        }
    
        Console.Clear();
        Console.CursorVisible = true;
    }

    static bool RunGame(DungeonDirector director)
    {
        var dungeonBuilder =
            new DungeonBuilder(InitialDungeonState.Filled, RoomWidth, RoomHeight, PlayerInitialPosition);
        var dungeon = director.Build(dungeonBuilder);
        var gc = new GameController(dungeon);

        Console.CursorVisible = false;
        Console.CancelKeyPress += (_, _) =>
        {
            Console.CursorVisible = true;
            Console.Clear();
        };
        Console.Clear();
        
        return 
    }
}