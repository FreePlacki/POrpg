namespace POrpg.Dungeon;

public class ConcreteDungeonBuilder : DungeonBuilder
{
    public ConcreteDungeonBuilder(InitialDungeonState initialState, int width, int height) : base(initialState, width, height)
    {
    }

    public override DungeonBuilder AddRandomPaths(int numPaths)
    {
        throw new NotImplementedException();
    }

    private ConcreteDungeonBuilder AddRoom(Position position, int width, int height)
    {
        for (var y = position.Y; y < position.Y + height; y++)
        {
            for (var x = position.X; x < position.X + width; x++)
            {
                Dungeon[(x, y)] = new FloorTile();
            }
        }

        return this;
    }
    
    public override DungeonBuilder AddRandomChambers(int numChambers)
    {
        const int minWidth = 3;
        const int minHeight = 3;
        var rng = new Random();
        for (var i = 0; i < numChambers; i++)
        {
            var position = new Position(rng.Next(Dungeon.Width), rng.Next(Dungeon.Height));
            var maxWidth = Dungeon.Width - position.X;
            var maxHeight = Dungeon.Height - position.Y;
            if (maxWidth < minWidth || maxHeight < minHeight)
            {
                i--;
                continue;
            }
            var width = rng.Next(minWidth, maxWidth);
            var height = rng.Next(minHeight, maxHeight);
            AddRoom(position, width, height);
        }
        return this;
    }

    public override DungeonBuilder AddCentralRoom()
    {
        var width = Dungeon.Width / 2;
        var height = Dungeon.Height / 2;
        var position = new Position((Dungeon.Width - width) / 2, (Dungeon.Height - height) / 2);
        return AddRoom(position, width, height);
    }

    public override Dungeon Build() => Dungeon;
}