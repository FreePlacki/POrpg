namespace POrpg.Items.Effects;

public abstract class WeaponEffect : Weapon
{
    protected readonly Weapon Weapon;

    protected WeaponEffect(Weapon weapon)
    {
        Weapon = weapon;
    }
}