using POrpg.Items;
using POrpg.Items.Weapons;

namespace POrpg.Combat;

public interface IAttackVisitor
{
    (int damage, int defense) Visit(HeavyWeapon weapon);
    (int damage, int defense) Visit(LightWeapon weapon);
    (int damage, int defense) Visit(MagicWeapon weapon);
    (int damage, int defense) Visit(Item nonWeapon);
}

public class NormalAttackVisitor : IAttackVisitor
{
    private readonly Attributes _attributes;

    public NormalAttackVisitor(Attributes attributes)
    {
        _attributes = attributes;
    }

    public (int damage, int defense) Visit(HeavyWeapon weapon) =>
        (weapon.Damage + _attributes[Attribute.Strength] + _attributes[Attribute.Aggression],
            _attributes[Attribute.Strength] + _attributes[Attribute.Luck]);

    public (int damage, int defense) Visit(LightWeapon weapon) =>
        (weapon.Damage + _attributes[Attribute.Dexterity] + _attributes[Attribute.Luck],
            _attributes[Attribute.Dexterity] + _attributes[Attribute.Luck]);

    public (int damage, int defense) Visit(MagicWeapon weapon) =>
        (1, _attributes[Attribute.Dexterity] + _attributes[Attribute.Luck]);

    public (int damage, int defense) Visit(Item nonWeapon) =>
        (0, _attributes[Attribute.Dexterity]);
}

public class StealthAttackVisitor : IAttackVisitor
{
    private readonly Attributes _attributes;

    public StealthAttackVisitor(Attributes attributes)
    {
        _attributes = attributes;
    }

    public (int damage, int defense) Visit(HeavyWeapon weapon)
    {
        int baseDamage = weapon.Damage + _attributes[Attribute.Strength] +
                         _attributes[Attribute.Aggression];
        return (baseDamage / 2, _attributes[Attribute.Strength]);
    }

    public (int damage, int defense) Visit(LightWeapon weapon)
    {
        int baseDamage = weapon.Damage + _attributes[Attribute.Dexterity] + _attributes[Attribute.Luck];
        return (baseDamage * 2, _attributes[Attribute.Dexterity]);
    }

    public (int damage, int defense) Visit(MagicWeapon weapon) => (1, 0);

    public (int damage, int defense) Visit(Item nonWeapon) => (0, 0);
}

public class MagicAttackVisitor : IAttackVisitor
{
    private readonly Attributes _attributes;

    public MagicAttackVisitor(Attributes attributes)
    {
        _attributes = attributes;
    }

    public (int damage, int defense) Visit(HeavyWeapon weapon) => (1, _attributes[Attribute.Luck]);

    public (int damage, int defense) Visit(LightWeapon weapon) => (1, _attributes[Attribute.Luck]);

    public (int damage, int defense) Visit(MagicWeapon weapon) =>
        (weapon.Damage + _attributes[Attribute.Wisdom],
            _attributes[Attribute.Wisdom] * 2);

    public (int damage, int defense) Visit(Item nonWeapon) => (0, _attributes[Attribute.Luck]);
}