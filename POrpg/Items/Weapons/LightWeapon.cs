using POrpg.Combat;

namespace POrpg.Items.Weapons;

public abstract class LightWeapon : Weapon
{
    public override int Accept(IAttackVisitor visitor) => visitor.Visit(this);
}
