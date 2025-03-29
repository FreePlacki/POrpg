using POrpg.Effects;

namespace POrpg.Items.Potions;

public class LuckPotion : Potion
{
    public LuckPotion(int? duration = 4) : base(duration)
    {
    }

    public override string Name => "Luck Potion";
    public override void Use(Player player)
    {
        _ = new LuckEffect(player, duration: Duration);
    }
}