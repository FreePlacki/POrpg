using POrpg.Inventory;

namespace POrpg.Items.Effects.WeaponEffects;

public class TwoHandedWeapon : WeaponEffect
{
    public TwoHandedWeapon(Weapon weapon) : base(weapon)
    {
    }

    public override string Name => $"Two-Handed {Weapon.Name}";
    public override int Damage => Weapon.Damage;
    
    public override Attributes? Attributes => Weapon.Attributes;
    public override EquipmentSlotType EquipmentSlotType => EquipmentSlotType.BothHands;
}