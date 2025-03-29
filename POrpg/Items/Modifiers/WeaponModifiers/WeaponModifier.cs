using POrpg.ConsoleHelpers;
using POrpg.Inventory;
using POrpg.Items.Weapons;

namespace POrpg.Items.Effects.WeaponEffects;

public abstract class WeaponModifier : Weapon
{
    protected readonly Weapon Weapon;

    protected WeaponModifier(Weapon weapon)
    {
        Weapon = weapon;
    }
    
    public override string Symbol => new StyledText(Weapon.Symbol, Styles.Effect).Text;
    
    public override Attributes? Attributes => Weapon.Attributes;
    public override EquipmentSlotType EquipmentSlotType => Weapon.EquipmentSlotType;
}