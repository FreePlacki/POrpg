using POrpg.Effects;

namespace POrpg.Items.Potions;

public class HealthPotion : Potion
{
    public HealthPotion(int? duration = null) : base(duration)
    {
    }

    public override string Name => "Health Potion";

    public override void Use(Dungeon.Dungeon dungeon, Player player)
    {
        _ = new HealEffect(dungeon, player);
    }
}