using System.Text;
using POrpg.ConsoleUtils;
using POrpg.Inventory;

namespace POrpg.Items.Weapons;

public abstract class Weapon : Item
{
    public override EquipmentSlotType EquipmentSlotType => EquipmentSlotType.LeftHand | EquipmentSlotType.RightHand;
    public abstract int Damage { get; }

    public override string Symbol => new StyledText(Name.First().ToString(), Styles.Weapon).ToString();

    public override string Description
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