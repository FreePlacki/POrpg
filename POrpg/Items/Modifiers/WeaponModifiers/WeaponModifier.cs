using POrpg.ConsoleHelpers;
using POrpg.Inventory;
using POrpg.Items.Weapons;

namespace POrpg.Items.Modifiers.WeaponModifiers;

public abstract class WeaponModifier : Weapon
{
    protected readonly Weapon Weapon;

    protected WeaponModifier(Weapon weapon)
    {
        Weapon = weapon;
    }
    
    public override string Symbol => new StyledText(Weapon.Symbol, Styles.Effect).ToString();
    
    public override Attributes? Attributes => Weapon.Attributes;
    public override EquipmentSlotType EquipmentSlotType => Weapon.EquipmentSlotType;
}