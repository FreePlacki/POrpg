using System.Text;
using POrpg.ConsoleHelpers;
using POrpg.Inventory;

namespace POrpg.Items;

public abstract class Weapon : Item
{
    public override string Symbol => new StyledText("W", Style.Cyan).Text;
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