using POrpg.Effects;

namespace POrpg.Items.Potions;

public class HealthPotion : Potion
{
    public HealthPotion(int? duration = null) : base(duration)
    {
    }

    public override string Name => "Health Potion";

    public override void Use(Dungeon.Dungeon dungeon, int playerId)
    {
        dungeon.TurnManager.RegisterObserver(new HealEffect(playerId));
    }
}