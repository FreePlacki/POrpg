namespace POrpg.Items.Effects;

public abstract class WeaponEffect : IWeapon
{
    protected readonly IWeapon Weapon;

    public WeaponEffect(IWeapon weapon)
    {
        Weapon = weapon;
    }
    
    public string Symbol => Weapon.Symbol;
    public abstract string Name { get; }
    public virtual int Damage => Weapon.Damage;
    Attributes? IItem.Attributes => Weapon.Attributes;
    bool IItem.IsTwoHanded => Weapon.IsTwoHanded;
}