using POrpg.Inventory;
using POrpg.Items.Weapons;

namespace POrpg.Items.Effects.WeaponEffects;

public abstract class WeaponEffect : Weapon
{
    protected readonly Weapon Weapon;

    protected WeaponEffect(Weapon weapon)
    {
        Weapon = weapon;
    }
    
    public override Attributes? Attributes => Weapon.Attributes;
    public override EquipmentSlotType EquipmentSlotType => Weapon.EquipmentSlotType;
}