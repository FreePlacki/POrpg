namespace POrpg;

public class EmptyTile : IDrawable
{
    public string Symbol => " ";
    public string Name => "Empty Tile";
}

public class WallTile : IDrawable
{
    public string Symbol => "\u2588";
    public string Name => "Wall";
    public bool IsPassable => false;
}
