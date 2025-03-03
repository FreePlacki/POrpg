using POrpg.ConsoleHelpers;
using POrpg.Inventory;
using POrpg.Items;

namespace POrpg;

public class Player : IDrawable
{
    public string Symbol => new StyledText("¶", Style.Magenta).Text;
    public string Name => "Player";

    public Position Position;
    public int Coins { get; set; }
    public int Gold { get; set; }
    public Inventory.Inventory Inventory { get; } = new();

    public Attributes Attributes = new(
        new()
        {
            { Attribute.Strength, 15 },
            { Attribute.Dexterity, 8 },
            { Attribute.Health, 3 },
            { Attribute.Luck, -13 },
            { Attribute.Aggression, 10 },
            { Attribute.Wisdom, 42 }
        });

    public Player(Position position) => Position = position;

    public void PickUp(IItem item)
    {
        if (item.OnPickUp(this)) return;
        Inventory.AppendToBackpack(item);
        // TODO: do all items affect attributes or only equipped ones?
        Attributes = (Attributes + item.Attributes)!;
    }

    public IItem Drop(InventorySlot slot)
    {
        var item = Inventory.RemoveAt(slot)!;
        Attributes = (Attributes - item.Attributes)!;
        return item;
    }
}