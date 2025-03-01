using POrpg.ConsoleHelpers;
using POrpg.Items;

namespace POrpg;

public class Player : IDrawable
{
    public string Symbol => new StyledText("¶", Style.Magenta).Text;
    public string Name => "Player";

    public Position Position;
    public int Coins { get; set; }
    public int Gold { get; set; }
    
    public List<IItem> Inventory { get; } = [];

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
        // TODO: do all items affect attributes or only equipped ones?
        Inventory.Add(item);
        Attributes = (Attributes + item.Attributes)!;
    }

    public IItem Drop(int index)
    {
        var item = Inventory[index];
        Attributes = (Attributes - item.Attributes)!;
        Inventory.RemoveAt(index);
        return item;
    }
}