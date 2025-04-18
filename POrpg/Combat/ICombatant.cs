using POrpg.Combat;

namespace POrpg.Items.Weapons;

public interface ICombatant
{
    public Item EquippedWeapon { get; }

    public (int damage, int defense) PerformAttack(IAttackVisitor attackVisitor)
    {
        return EquippedWeapon.Accept(attackVisitor);
    }
}