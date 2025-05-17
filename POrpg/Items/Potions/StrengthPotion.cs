using POrpg.Effects;

namespace POrpg.Items.Potions;

public class StrengthPotion : Potion
{
    public StrengthPotion(int? duration = 5) : base(duration)
    {
    }

    public override string Name => "Strength Potion";

    public override void Use(Dungeon.Dungeon dungeon, Player player)
    {
        _ = new PowerEffect(dungeon, player, Duration);
    }
}