using POrpg.ConsoleUtils;
using POrpg.Inventory;

namespace POrpg.Items.Modifiers;

public abstract class Modifier : Item
{
    public Item Item { get; set; }

    protected Modifier(Item item)
    {
        Item = item;
    }

    public override string Symbol => new StyledText(Item.Symbol, Styles.Effect).ToString();

    public override Attributes? Attributes => Item.Attributes;
    public override EquipmentSlotType EquipmentSlotType => Item.EquipmentSlotType;
}