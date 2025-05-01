using System.Text.Json.Serialization;
using POrpg.ConsoleHelpers;
using POrpg.InputHandlers;
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
    public int Width { get; set; }
    public int Height { get; set; }
    public bool ShouldQuit { get; set; }
    public Item? CurrentItem => CurrentTile.CurrentItem;
    public Item? SelectedItem => Player.SelectedSlot != null ? Player.Inventory[Player.SelectedSlot] : null;
    public Tile CurrentTile => this[Player.Position];
    public bool IsChoosingAttack { get; set; }

    public Player Player { get; set; }
    public Tile[,] Tiles { get; set; }

    public Tile this[Position p]
    {
        get => Tiles[p.Y, p.X];
        set => Tiles[p.Y, p.X] = value;
    }

    [JsonConstructor]
    public Dungeon(int width, int height, Tile[,] tiles, Player player)
    {
        (Width, Height) = (width, height);
        Tiles           = tiles;
        Player          = player;
    }

    public Dungeon(InitialDungeonState initialState, int width, int height, Position playerInitialPosition)
    {
        (Width, Height) = (width, height);
        Tiles           = new Tile[height, width];
        Player          = new Player(playerInitialPosition);

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

        this[playerInitialPosition] = new FloorTile();
    }

    public void ProcessInput(InputHandler inputHandler, ConsoleKeyInfo keyInfo)
    {
        var command = inputHandler.HandleInput(this, keyInfo);
        command.Execute();
        if (command.Description != null)
            ConsoleHelper.GetInstance().AddNotification(command.Description);
        if (command.AdvancesTurn)
            TurnManager.GetInstance().NextTurn();
    }

    public bool TryMovePlayer(Position direction)
    {
        Player.LookingAt = null;
        var newPos = Player.Position + direction;
        if (CanMoveTo(newPos))
        {
            Player.Position = newPos;
            return true;
        }

        if (IsInBounds(newPos))
        {
            Player.LookingAt = this[newPos];
        }

        return false;
    }

    public Item? TryPickUpItem()
    {
        if (CurrentItem == null || Player.Inventory.Backpack.IsFull) return null;
        var item = CurrentItem;
        Player.PickUp(CurrentItem);
        CurrentTile.RemoveCurrentItem();
        return item;
    }

    private Item? TryDropItem(InventorySlot slot)
    {
        if (Player.Inventory[slot] == null) return null;

        var item = Player.Drop(slot);
        CurrentTile.Add(item);
        return item;
    }

    public Item? TryDropItem()
    {
        if (Player.SelectedSlot == null) return null;
        var item = TryDropItem(Player.SelectedSlot);
        Player.SelectedSlot = null;
        return item;
    }

    public bool TryDropAllItems()
    {
        var dropped = false;

        while (TryDropItem(new BackpackSlot(0)) != null)
            dropped = true;
        if (TryDropItem(new EquipmentSlot(EquipmentSlotType.LeftHand)) != null)
            dropped = true;
        if (TryDropItem(new EquipmentSlot(EquipmentSlotType.RightHand)) != null)
            dropped = true;
        if (TryDropItem(new EquipmentSlot(EquipmentSlotType.BothHands)) != null)
            dropped = true;

        return dropped;
    }

    public bool TrySelectItem(InventorySlot slot)
    {
        if (!slot.IsValid(Player.Inventory)) return false;

        var normSlot = slot.Normalize(Player.Inventory, Player.SelectedSlot);

        // deselect when selecting the current active slot
        if (Player.SelectedSlot == normSlot)
        {
            Player.SelectedSlot = null;
            return true;
        }

        if (Player.SelectedSlot == null || Player.Inventory[Player.SelectedSlot] == null)
        {
            Player.SelectedSlot = normSlot;
            return true;
        }

        Player.Inventory.Swap(Player.SelectedSlot, slot);
        slot                = slot.Normalize(Player.Inventory, Player.SelectedSlot);
        Player.SelectedSlot = slot;

        return true;
    }

    public bool TryMoveToBackpack()
    {
        if (Player.SelectedSlot == null || Player.Inventory[Player.SelectedSlot] == null ||
            !Player.SelectedSlot.CanMoveToBackpack || Player.Inventory.Backpack.IsFull) return false;
        Player.SelectedSlot.MoveToBackpack(Player.Inventory);
        Player.SelectedSlot = null;

        return true;
    }

    public Item? TryUseItem()
    {
        if (Player.SelectedSlot == null) return null;
        if (Player.Inventory[Player.SelectedSlot] is not IUsable item) return null;

        item.Use(Player);
        var res = Player.Drop(Player.SelectedSlot);
        return res;
    }

    public void PerformAttack(IAttackVisitor visitor)
    {
        var damage = 0;
        var defense = 0;
        if (Player.SelectedSlot is EquipmentSlot)
        {
            (damage, defense) = Player.Inventory[Player.SelectedSlot]?.Accept(visitor) ?? (0, 0);
        }

        damage = Player.LookingAt!.Enemy!.DealDamage(damage);
        ConsoleHelper.GetInstance().AddNotification($"Dealt {damage} damage to {Player.LookingAt.Name}");
        if (Player.LookingAt.Enemy.Health <= 0)
        {
            ConsoleHelper.GetInstance().AddNotification($"{Player.LookingAt.Name} has been slain");
            Player.LookingAt.Enemy = null;
            return;
        }

        damage = Player.DealDamage(Player.LookingAt.Enemy.Damage, defense);
        ConsoleHelper.GetInstance().AddNotification($"{Player.LookingAt.Name} hits back for {damage}");
    }

    public void CycleItems(bool reverse) => CurrentTile.CycleItems(reverse);

    public bool IsInBounds(Position p) => p.X >= 0 && p.X < Width && p.Y >= 0 && p.Y < Height;

    private bool CanMoveTo(Position p) => IsInBounds(p) && this[p].IsPassable;
    //
    // public IEnumerator<Tile> GetEnumerator()
    // {
    //     for (var y = 0; y < Height; y++)
    //     {
    //         for (var x = 0; x < Width; x++)
    //         {
    //             yield return _tiles[y, x];
    //         }
    //     }
    // }
    //
    // IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}