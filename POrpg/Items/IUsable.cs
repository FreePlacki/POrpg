namespace POrpg.Items;

public interface IUsable
{
    void Use(Dungeon.Dungeon dungeon, int playerId);
}