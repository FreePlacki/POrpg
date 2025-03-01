namespace POrpg;

class Program
{
    static void Main(string[] args)
    {
        const int roomWidth = 40;
        const int roomHeight = 20;
        var room = new Room(roomWidth, roomHeight);

        while (true)
        {
            Console.Clear();
            room.Draw();
            var input = Console.ReadKey(true);
            room.ProcessInput(input);
        }
    }
}