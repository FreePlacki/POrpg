using POrpg.Items.Weapons;

namespace POrpg.Items.Modifiers.WeaponModifiers;

public class Powerful : WeaponModifier
{
    public Powerful(Weapon weapon) : base(weapon)
    {
    }

    public override string Name => $"{Weapon.Name} (Powerful)";
    public override int Damage => Weapon.Damage + 5;
}