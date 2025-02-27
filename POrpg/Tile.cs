namespace POrpg;

public class EmptyTile : IDrawable
{
    public override string ToString() => " ";
    public string Description => "Empty Tile";
}

public class WallTile : IDrawable
{
    public override string ToString() => "\u2588";
    public string Description => "Wall";
    public bool IsPassable => false;
}
