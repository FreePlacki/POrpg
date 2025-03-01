namespace POrpg.Items.Effects;

public class Powerful : WeaponEffect
{
    public Powerful(IWeapon weapon) : base(weapon)
    {
    }

    public override string Name => $"{Weapon.Name} (Powerful)";
    public override int Damage => Weapon.Damage + 5;
}