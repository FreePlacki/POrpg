using POrpg.ConsoleHelpers;
using POrpg.Inventory;

namespace POrpg.Items.Effects;

public abstract class Modifier : Item
{
    protected readonly Item Item;

    protected Modifier(Item item)
    {
        Item = item;
    }
    public override string Symbol => new StyledText(Item.Symbol, Styles.Effect).Text;
    
    public override Attributes? Attributes => Item.Attributes;
    public override EquipmentSlotType EquipmentSlotType => Item.EquipmentSlotType;
}