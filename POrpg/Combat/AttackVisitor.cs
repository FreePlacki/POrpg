using POrpg.Items;
using POrpg.Items.Weapons;

namespace POrpg.Combat;

public interface IAttackVisitor
{
    int Visit(HeavyWeapon weapon);
    int Visit(LightWeapon weapon);
    int Visit(MagicWeapon weapon);
    int Visit(Item nonWeapon);
}

public class NormalAttackVisitor : IAttackVisitor
{
    private readonly Attributes _attributes;

    public NormalAttackVisitor(Attributes attributes)
    {
        _attributes = attributes;
    }

    public int Visit(HeavyWeapon weapon) =>
        weapon.Damage + _attributes[Attribute.Strength] / 2 + _attributes[Attribute.Aggression] / 2;

    public int Visit(LightWeapon weapon) =>
        weapon.Damage + _attributes[Attribute.Dexterity] / 2 + _attributes[Attribute.Luck] / 2;

    public int Visit(MagicWeapon weapon) => 1;

    public int Visit(Item nonWeapon) => 0;
}

public class StealthAttackVisitor : IAttackVisitor
{
    private readonly Attributes _attributes;

    public StealthAttackVisitor(Attributes attributes)
    {
        _attributes = attributes;
    }

    public int Visit(HeavyWeapon weapon)
    {
        int baseDamage = weapon.Damage + _attributes[Attribute.Strength] / 2 +
                         _attributes[Attribute.Aggression] / 2;
        return baseDamage / 2;
    }

    public int Visit(LightWeapon weapon)
    {
        int baseDamage = weapon.Damage + _attributes[Attribute.Dexterity] / 2 + _attributes[Attribute.Luck] / 2;
        return baseDamage * 2;
    }

    public int Visit(MagicWeapon weapon) => 1;

    public int Visit(Item nonWeapon) => 0;
}

public class MagicAttackVisitor : IAttackVisitor
{
    private readonly Attributes _attributes;

    public MagicAttackVisitor(Attributes attributes)
    {
        _attributes = attributes;
    }

    public int Visit(HeavyWeapon weapon) => 1;

    public int Visit(LightWeapon weapon) => 1;

    public int Visit(MagicWeapon weapon) => weapon.Damage + _attributes[Attribute.Wisdom] * 2;

    public int Visit(Item nonWeapon) => 0;
}