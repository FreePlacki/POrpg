using POrpg.Combat;

namespace POrpg.Items.Weapons;

public abstract class MagicWeapon : Weapon
{
    public override int Accept(IAttackVisitor visitor) => visitor.Visit(this);
}
