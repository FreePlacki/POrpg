using POrpg.ConsoleHelpers;
using POrpg.Inventory;

namespace POrpg.Items.Modifiers;

public abstract class Modifier : Item
{
    protected readonly Item Item;

    protected Modifier(Item item)
    {
        Item = item;
    }
    public override string Symbol => new StyledText(Item.Symbol, Styles.Effect).ToString();
    
    public override Attributes? Attributes => Item.Attributes;
    public override EquipmentSlotType EquipmentSlotType => Item.EquipmentSlotType;
}