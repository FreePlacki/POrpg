using POrpg.Items.Weapons;

namespace POrpg.Items.Modifiers.WeaponModifiers;

public class UnluckyWeapon : WeaponModifier
{
    public UnluckyWeapon(Weapon weapon) : base(weapon)
    {
    }

    public override string Name => $"{Weapon.Name} (Unlucky)";
    public override int Damage => Weapon.Damage;

    public override Attributes? Attributes => new Attributes(new()
    {
        { Attribute.Luck, -5 }
    }) + Weapon.Attributes;
}