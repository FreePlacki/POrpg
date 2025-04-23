namespace POrpg.Items.Weapons;

public abstract class HeavyWeapon : Weapon
{
    public override (int damage, int defense) Accept(IAttackVisitor visitor) => visitor.Visit(this);
}