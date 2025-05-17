using POrpg.ConsoleUtils;
using POrpg.Items.Weapons;

namespace POrpg.Items.Modifiers.WeaponModifiers;

public class LegendaryWeapon : WeaponModifier
{
    public LegendaryWeapon(Weapon weapon) : base(weapon)
    {
    }

    public override string Name => $"{Weapon.Name} ({new StyledText("Legendary", Style.Rainbow)})";

    public override Attributes Attributes =>
        new Attributes(new Dictionary<Attribute, int>
        {
            { Attribute.Strength, 5 },
            { Attribute.Dexterity, 5 },
            { Attribute.Health, 5 },
            { Attribute.Luck, 5 },
            { Attribute.Aggression, 5 },
            { Attribute.Wisdom, 5 }
        }) + Weapon.Attributes;

    public override int Damage => Weapon.Damage;
}