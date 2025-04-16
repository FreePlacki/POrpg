using POrpg.Combat;

namespace POrpg.Items.Weapons;

public interface ICombatant
{
    public Item EquippedWeapon { get; }

    public int PerformAttack(IAttackVisitor attackVisitor)
    {
        return EquippedWeapon.Accept(attackVisitor);
    }
}