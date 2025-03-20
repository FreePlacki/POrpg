using POrpg.ConsoleHelpers;
using POrpg.Inventory;

namespace POrpg.Items.Effects;

public abstract class Effect : Item
{
    protected readonly Item Item;

    protected Effect(Item item)
    {
        Item = item;
    }
    public override string Symbol => new StyledText(Item.Symbol, Style.Bold).Text;
    
    public override Attributes? Attributes => Item.Attributes;
    public override EquipmentSlotType EquipmentSlotType => Item.EquipmentSlotType;
}