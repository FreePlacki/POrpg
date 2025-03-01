namespace POrpg;

public class EmptyTile : IDrawable
{
    public char Symbol => ' ';
    public string Name => "Empty Tile";
}

public class WallTile : IDrawable
{
    public char Symbol => '\u2588';
    public string Name => "Wall";
    public bool IsPassable => false;
}
