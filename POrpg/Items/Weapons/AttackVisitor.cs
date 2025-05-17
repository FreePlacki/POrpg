namespace POrpg.Items.Weapons;

public interface IAttackVisitor
{
    (int damage, int defense) Visit(HeavyWeapon weapon);
    (int damage, int defense) Visit(LightWeapon weapon);
    (int damage, int defense) Visit(MagicWeapon weapon);
    (int damage, int defense) Visit(Item nonWeapon);
}

public class NormalAttackVisitor : IAttackVisitor
{
    public Attributes Attributes { get; }

    public NormalAttackVisitor(Attributes attributes)
    {
        Attributes = attributes;
    }

    public (int damage, int defense) Visit(HeavyWeapon weapon) =>
        (weapon.Damage + Attributes[Attribute.Strength] + Attributes[Attribute.Aggression],
            Attributes[Attribute.Strength] + Attributes[Attribute.Luck]);

    public (int damage, int defense) Visit(LightWeapon weapon) =>
        (weapon.Damage + Attributes[Attribute.Dexterity] + Attributes[Attribute.Luck],
            Attributes[Attribute.Dexterity] + Attributes[Attribute.Luck]);

    public (int damage, int defense) Visit(MagicWeapon weapon) =>
        (1, Attributes[Attribute.Dexterity] + Attributes[Attribute.Luck]);

    public (int damage, int defense) Visit(Item nonWeapon) =>
        (0, Attributes[Attribute.Dexterity]);
}

public class StealthAttackVisitor : IAttackVisitor
{
    public Attributes Attributes { get; }

    public StealthAttackVisitor(Attributes attributes)
    {
        Attributes = attributes;
    }

    public (int damage, int defense) Visit(HeavyWeapon weapon)
    {
        int baseDamage = weapon.Damage + Attributes[Attribute.Strength] +
                         Attributes[Attribute.Aggression];
        return (baseDamage / 2, Attributes[Attribute.Strength]);
    }

    public (int damage, int defense) Visit(LightWeapon weapon)
    {
        int baseDamage = weapon.Damage + Attributes[Attribute.Dexterity] + Attributes[Attribute.Luck];
        return (baseDamage * 2, Attributes[Attribute.Dexterity]);
    }

    public (int damage, int defense) Visit(MagicWeapon weapon) => (1, 0);

    public (int damage, int defense) Visit(Item nonWeapon) => (0, 0);
}

public class MagicAttackVisitor : IAttackVisitor
{
    public Attributes Attributes { get; }

    public MagicAttackVisitor(Attributes attributes)
    {
        Attributes = attributes;
    }

    public (int damage, int defense) Visit(HeavyWeapon weapon) => (1, Attributes[Attribute.Luck]);

    public (int damage, int defense) Visit(LightWeapon weapon) => (1, Attributes[Attribute.Luck]);

    public (int damage, int defense) Visit(MagicWeapon weapon) =>
        (weapon.Damage + Attributes[Attribute.Wisdom],
            Attributes[Attribute.Wisdom] * 2);

    public (int damage, int defense) Visit(Item nonWeapon) => (0, Attributes[Attribute.Luck]);
}