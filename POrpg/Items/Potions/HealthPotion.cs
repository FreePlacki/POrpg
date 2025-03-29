using POrpg.Effects;

namespace POrpg.Items.Potions;

public class HealthPotion : Potion
{
    public HealthPotion(int? duration = null) : base(duration)
    {
    }

    public override string Name => "Health Potion";
    public override void Use(Player player)
    {
        _ = new HealEffect(player);
    }
}