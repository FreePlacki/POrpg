using POrpg.Effects;

namespace POrpg.Items.Potions;

public class LuckPotion : Potion
{
    public LuckPotion(int? duration = 4) : base(duration)
    {
    }

    public override string Name => "Luck Potion";

    public override void Use(Dungeon.Dungeon dungeon, int playerId)
    {
        dungeon.TurnManager.RegisterObserver(new LuckEffect(playerId));
    }
}