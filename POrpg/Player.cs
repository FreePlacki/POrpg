using POrpg.ConsoleHelpers;
using POrpg.Dungeon;
using POrpg.Effects;
using POrpg.Inventory;
using POrpg.Items;
using POrpg.Items.Weapons;

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

    private Attributes _attributes = new(
        new()
        {
            { Attribute.Strength, 10 },
            { Attribute.Dexterity, 10 },
            { Attribute.Health, 10 },
            { Attribute.Luck, 10 },
            { Attribute.Aggression, 10 },
            { Attribute.Wisdom, 10 }
        });

    public List<Effect> Effects { get; } = [];

    public Attributes Attributes =>
        _attributes +
        Inventory.TotalAttributes +
        Effects.Aggregate(new Attributes(new()), (acc, e) => acc + e.Attributes);

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

    public void AddEffect(Effect effect)
    {
        if (effect.IsPermanent)
            _attributes += effect.Attributes;
        else
            Effects.Add(effect);
    }

    public void RemoveEffect(Effect effect) => Effects.Remove(effect);
}