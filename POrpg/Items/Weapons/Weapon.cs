using System.Text;
using POrpg.Combat;
using POrpg.ConsoleHelpers;
using POrpg.Inventory;

namespace POrpg.Items.Weapons;

public abstract class Weapon : Item
{
    public override string Symbol => new StyledText("W", Styles.Weapon).Text;
    public override EquipmentSlotType EquipmentSlotType => EquipmentSlotType.LeftHand | EquipmentSlotType.RightHand;
    public abstract int Damage { get; }

    public override string? Description
    {
        get
        {
            var sb = new StringBuilder($"Damage: {Damage}");
            if (base.Description != null)
                sb.Append($"\n{base.Description}");
            return sb.ToString();
        }
    }
}