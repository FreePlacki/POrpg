using System.Text.Json.Serialization;
using POrpg.ConsoleUtils;
using POrpg.Dungeon;
using POrpg.Effects;
using POrpg.Inventory;
using POrpg.Items;

namespace POrpg;

public class Player : IDrawable
{
    public int Id { get; }
    public string Symbol => new StyledText(Id.ToString(), Styles.Player).ToString();
    public string Name => $"Player {new StyledText(Id.ToString(), Styles.Player)}";
    public string? Description => null;

    public Position Position { get; set; }
    public int Coins { get; set; }
    public int Gold { get; set; }
    public Inventory.Inventory Inventory { get; }
    public InventorySlot? SelectedSlot { get; set; }
    public Tile? LookingAt { get; set; }

    [JsonInclude] private Attributes _attributes = new(
        new()
        {
            { Attribute.Strength, 10 },
            { Attribute.Dexterity, 10 },
            { Attribute.Health, 10 },
            { Attribute.Luck, 10 },
            { Attribute.Aggression, 10 },
            { Attribute.Wisdom, 10 }
        });

    public List<Effect> Effects { get; set; }

    [JsonIgnore]
    public Attributes Attributes =>
        Effects.Aggregate(_attributes + Inventory.TotalAttributes, (acc, e) => acc + e.Attributes);

    public Player(Position position, int id)
    {
        Position = position;
        Id = id;
        Inventory = new();
        Effects = [];
    }

    [JsonConstructor]
    public Player(Position position, int id, Inventory.Inventory inventory, List<Effect> effects)
    {
        Position = position;
        Id = id;
        Inventory = inventory;
        Effects = effects;
    }

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

    public int DealDamage(int damage, int defense)
    {
        int dmg = Math.Max(0, damage - defense);
        _attributes[Attribute.Health] -= dmg;
        return dmg;
    }
}