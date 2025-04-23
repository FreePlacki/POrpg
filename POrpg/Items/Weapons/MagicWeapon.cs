namespace POrpg.Items.Weapons;

public abstract class MagicWeapon : Weapon
{
    public override (int damage, int defense) Accept(IAttackVisitor visitor) => visitor.Visit(this);
}
