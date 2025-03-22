using POrpg.ConsoleHelpers;
using POrpg.Dungeon;
using POrpg.Inventory;
using POrpg.Items;

namespace POrpg;

public class Player : IDrawable
{
    public string Symbol => new StyledText("¶", Styles.Player).Text;
    public string Name => "Player";
    public string? Description => null;

    public Position Position;
    public int Coins { get; set; }
    public int Gold { get; set; }
    public Inventory.Inventory Inventory { get; } = new();

    private readonly Attributes _attributes = new(
        new()
        {
            { Attribute.Strength, 15 },
            { Attribute.Dexterity, 8 },
            { Attribute.Health, 3 },
            { Attribute.Luck, -13 },
            { Attribute.Aggression, 10 },
            { Attribute.Wisdom, 42 }
        });

    public Attributes Attributes => _attributes + Inventory.TotalAttributes;

    public Player(Position position) => Position = position;

    public void PickUp(Item item)
    {
        if (item.OnPickUp(this)) return;
        if (Inventory.Backpack.IsFull) return;
        Inventory.AppendToBackpack(item);
    }

    public Item Drop(InventorySlot slot)
    {
        var item = Inventory.RemoveAt(slot)!;
        return item;
    }
}