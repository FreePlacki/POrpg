using POrpg.Combat;

namespace POrpg.Items.Weapons;

public abstract class LightWeapon : Weapon
{
    public override (int damage, int defense) Accept(IAttackVisitor visitor) => visitor.Visit(this);
}
