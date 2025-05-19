using System.Text.Json.Serialization;
using POrpg.ConsoleUtils;
using POrpg.Inventory;
using POrpg.Items;
using POrpg.Items.Weapons;

namespace POrpg.Dungeon;

public record struct Position(int X, int Y)
{
    public static implicit operator Position((int x, int y) p) => new() { X = p.x, Y = p.y };
    public static Position operator +(Position a, Position b) => new(a.X + b.X, a.Y + b.Y);
}

public class Dungeon
{
    public int Width { get; }
    public int Height { get; }

    public Dictionary<int, Player> Players { get; } = [];
    public Tile[,] Tiles { get; }
    public TurnManager TurnManager { get; }

    public Tile this[Position p]
    {
        get => Tiles[p.Y, p.X];
        set => Tiles[p.Y, p.X] = value;
    }

    [JsonConstructor]
    public Dungeon(int width, int height, Tile[,] tiles, Dictionary<int, Player> players, TurnManager turnManager)
    {
        (Width, Height) = (width, height);
        Tiles = tiles;
        Players = players;
        TurnManager = turnManager;
    }

    public Dungeon(InitialDungeonState initialState, int width, int height)
    {
        (Width, Height) = (width, height);
        Tiles = new Tile[height, width];
        TurnManager = new TurnManager();

        for (var y = 0; y < Height; y++)
        {
            for (var x = 0; x < Width; x++)
            {
                this[(x, y)] = initialState switch
                {
                    InitialDungeonState.Empty => new FloorTile(),
                    InitialDungeonState.Filled => new WallTile(),
                    _ => throw new ArgumentOutOfRangeException(nameof(initialState), initialState, null)
                };
            }
        }
    }

    public void AddPlayer(int playerId)
    {
        var pos = new Position(0, 0);
        var rng = Random.Shared;
        while (!this[pos].IsPassable || Players.Values.Any(p => p.Position == pos))
            pos = (rng.Next(Width), rng.Next(Height));
        Players[playerId] = new Player(pos);
    }

    public bool TryMovePlayer(Position direction, int playerId)
    {
        var player = Players[playerId];

        // TODO: Looking at should be in the view
        player.LookingAt = null;
        var newPos = player.Position + direction;
        if (CanMoveTo(newPos))
        {
            player.Position = newPos;
            return true;
        }

        if (IsInBounds(newPos))
        {
            player.LookingAt = this[newPos];
        }

        return false;
    }

    public Item? TryPickUpItem(int playerId)
    {
        var player = Players[playerId];
        var item = this[player.Position].CurrentItem;
        if (item == null) return null;
        if (player.Inventory.Backpack.IsFull)
        {
            ConsoleHelper.GetInstance().AddNotification("Your backpack is full!");
            return null;
        }

        player.PickUp(item);
        // TODO: consider tile.current item to be in view (it might make sense to leave it like this -- once a player
        // shuffles items it's visible for the rest)
        this[player.Position].RemoveCurrentItem();
        return item;
    }

    private Item? TryDropItem(InventorySlot slot, int playerId)
    {
        var player = Players[playerId];
        if (player.Inventory[slot] == null) return null;

        var item = player.Drop(slot);
        this[player.Position].Add(item);
        return item;
    }

    public Item? TryDropItem(int playerId)
    {
        var player = Players[playerId];
        if (player.SelectedSlot == null) return null;
        var item = TryDropItem(player.SelectedSlot, playerId);
        player.SelectedSlot = null;
        return item;
    }

    public bool TryDropAllItems(int playerId)
    {
        var dropped = false;

        while (TryDropItem(new BackpackSlot(0), playerId) != null)
            dropped = true;
        if (TryDropItem(new EquipmentSlot(EquipmentSlotType.LeftHand), playerId) != null)
            dropped = true;
        if (TryDropItem(new EquipmentSlot(EquipmentSlotType.RightHand), playerId) != null)
            dropped = true;
        if (TryDropItem(new EquipmentSlot(EquipmentSlotType.BothHands), playerId) != null)
            dropped = true;

        return dropped;
    }

    public bool TrySelectItem(InventorySlot slot, int playerId)
    {
        var player = Players[playerId];
        if (!slot.IsValid(player.Inventory)) return false;

        var normSlot = slot.Normalize(player.Inventory, player.SelectedSlot);

        // deselect when selecting the current active slot
        if (player.SelectedSlot == normSlot)
        {
            player.SelectedSlot = null;
            return true;
        }

        if (player.SelectedSlot == null || player.Inventory[player.SelectedSlot] == null)
        {
            player.SelectedSlot = normSlot;
            return true;
        }

        player.Inventory.Swap(player.SelectedSlot, slot);
        slot = slot.Normalize(player.Inventory, player.SelectedSlot);
        player.SelectedSlot = slot;

        return true;
    }

    public bool TryMoveToBackpack(int playerId)
    {
        var player = Players[playerId];
        if (player.SelectedSlot == null || player.Inventory[player.SelectedSlot] == null ||
            !player.SelectedSlot.CanMoveToBackpack || player.Inventory.Backpack.IsFull) return false;
        player.SelectedSlot.MoveToBackpack(player.Inventory);
        player.SelectedSlot = null;

        return true;
    }

    public Item? TryUseItem(int playerId)
    {
        var player = Players[playerId];
        if (player.SelectedSlot == null) return null;
        if (player.Inventory[player.SelectedSlot] is not IUsable item) return null;

        item.Use(this, playerId);
        var res = player.Drop(player.SelectedSlot);
        return res;
    }

    public void PerformAttack(IAttackVisitor visitor, int playerId)
    {
        var player = Players[playerId];
        var damage = 0;
        var defense = 0;
        if (player.SelectedSlot is EquipmentSlot)
        {
            (damage, defense) = player.Inventory[player.SelectedSlot]?.Accept(visitor) ?? (0, 0);
        }

        damage = player.LookingAt!.Enemy!.DealDamage(damage);
        ConsoleHelper.GetInstance().AddNotification($"Dealt {damage} damage to {player.LookingAt.Name}");
        if (player.LookingAt.Enemy.Health <= 0)
        {
            ConsoleHelper.GetInstance().AddNotification($"{player.LookingAt.Name} has been slain");
            player.LookingAt.Enemy = null;
            return;
        }

        damage = player.DealDamage(player.LookingAt.Enemy.Damage, defense);
        ConsoleHelper.GetInstance().AddNotification($"{player.LookingAt.Name} hits back for {damage}");
    }

    public void CycleItems(bool reverse, int playerId) => this[Players[playerId].Position].CycleItems(reverse);

    public bool IsInBounds(Position p) => p.X >= 0 && p.X < Width && p.Y >= 0 && p.Y < Height;

    private bool CanMoveTo(Position p) => IsInBounds(p) && this[p].IsPassable;

    public void NextTurn()
    {
        TurnManager.NextTurn();
        foreach (var (id, player) in Players)
            player.Effects = TurnManager.Observers.Where(o => o.PlayerId == id).ToList();
    }
}