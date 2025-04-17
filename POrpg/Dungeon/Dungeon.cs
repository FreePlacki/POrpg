using System.Collections;
using System.Diagnostics;
using POrpg.ConsoleHelpers;
using POrpg.InputHandlers;
using POrpg.Inventory;
using POrpg.Items;

namespace POrpg.Dungeon;

public record struct Position(int X, int Y)
{
    public static implicit operator Position((int x, int y) p) => new() { X = p.x, Y = p.y };
    public static Position operator +(Position a, Position b) => new(a.X + b.X, a.Y + b.Y);
}

public class Dungeon : IEnumerable<Tile>
{
    public int Width { get; }
    public int Height { get; }
    public bool ShouldQuit { get; set; }
    public Item? CurrentItem => CurrentTile.CurrentItem;
    public Item? SelectedItem => _selectedSlot != null ? _player.Inventory[_selectedSlot] : null;
    public Tile CurrentTile => this[_player.Position];

    private readonly Tile[,] _tiles;
    private readonly Player _player;
    private Tile? LookingAt { get; set; }
    private InventorySlot? _selectedSlot;
    private InputHandler _inputHandler;

    public Tile this[Position p]
    {
        get => _tiles[p.Y, p.X];
        set => _tiles[p.Y, p.X] = value;
    }

    public Dungeon(InitialDungeonState initialState, int width, int height, Position playerInitialPosition)
    {
        (Width, Height) = (width, height);
        _tiles = new Tile[height, width];
        _player = new Player(playerInitialPosition);

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

    public void Draw(InputHandler inputHandler)
    {
        var sw = Stopwatch.StartNew();
        _inputHandler = inputHandler;
        var console = ConsoleHelper.GetInstance();

        for (var y = 0; y < Height; y++)
        {
            for (var x = 0; x < Width; x++)
            {
                Position pos = (x, y);
                if (pos == _player.Position)
                {
                    console.Write(new StyledText(_player.Symbol, Styles.Player));
                    continue;
                }

                console.Write(this[pos].Symbol);
            }

            console.WriteLine();
        }

        console.ChangeColumn(1);
        console.WriteLine($"{new StyledText("Turn:", Style.Underline).Text} {TurnManager.GetInstance().Turn}");
        console.WriteLine();
        DrawStats();
        console.HorizontalDivider();
        DrawInventory();
        console.HorizontalDivider();
        console.ChangeColumn(2);
        if (DrawActiveEffects())
            console.HorizontalDivider();
        if (DrawStandingOn())
            console.HorizontalDivider();
        if (DrawLookingAt())
            console.HorizontalDivider();

        console.ChangeColumn(0);
        console.PrintNotifications();
        console.WriteLine();

        foreach (var hint in _inputHandler.GetHints().Where(h => (h.Location & UiLocation.Bottom) != 0))
        {
            console.WriteHint(hint);
            console.Write(" ");
        }

        console.WriteLine();

        sw.Stop();
        console.WriteLine(new StyledText($"Frame time: {sw.Elapsed.Milliseconds} ms", Style.Faint));
    }

    private void DrawStats()
    {
        var console = ConsoleHelper.GetInstance();
        console.WriteLine(new StyledText("Player Stats:", Style.Underline));
        foreach (var attribute in _player.Attributes)
        {
            console.Write($"{attribute.Key,-15} ");
            console.WriteLine(new StyledText(attribute.Value.ToString(), Style.Gradient));
        }

        console.WriteLine();
        console.WriteLine($"{"Coins",-15} {new StyledText(_player.Coins.ToString(), Styles.Money).Text}");
        console.WriteLine($"{"Gold",-15} {new StyledText(_player.Gold.ToString(), Styles.Money).Text}");
    }

    private void DrawInventory()
    {
        var console = ConsoleHelper.GetInstance();
        console.WriteLine(new StyledText("Inventory:", Style.Underline));

        if (_selectedSlot == new EquipmentSlot(EquipmentSlotType.LeftHand))
        {
            console.Write($"{new StyledText("L", Styles.Player).Text}. ");
            var leftHand = _player.Inventory[new EquipmentSlot(EquipmentSlotType.LeftHand)];
            if (leftHand != null)
            {
                console.WriteLine(leftHand.Name);
                console.WriteLine(leftHand.Description);
                foreach (var hint in _inputHandler.GetHints().Where(h => (h.Location & UiLocation.Inventory) != 0))
                    console.WriteHintLine(hint);
            }
            else
            {
                console.WriteLine("Empty");
            }

            console.Write($"{new StyledText(new StyledText("R", Styles.Player), Style.Faint).Text}. ");
            var rightHand = _player.Inventory.Equipment.RightHand;
            console.WriteLine(rightHand != null ? rightHand.Name : "Empty");
        }
        else if (_selectedSlot == new EquipmentSlot(EquipmentSlotType.RightHand))
        {
            console.Write($"{new StyledText(new StyledText("L", Styles.Player), Style.Faint).Text}. ");
            var leftHand = _player.Inventory.Equipment.LeftHand;
            console.WriteLine(leftHand != null ? leftHand.Name : "Empty");

            console.Write($"{new StyledText("R", Styles.Player).Text}. ");
            var rightHand = _player.Inventory.Equipment.RightHand;
            if (rightHand != null)
            {
                console.WriteLine(rightHand.Name);
                console.WriteLine(rightHand.Description);
                foreach (var hint in _inputHandler.GetHints().Where(h => (h.Location & UiLocation.Inventory) != 0))
                    console.WriteHintLine(hint);
            }
            else
            {
                console.WriteLine("Empty");
            }
        }
        else
        {
            if (_player.Inventory.Equipment.BothHands != null)
            {
                var item = _player.Inventory.Equipment.BothHands;
                if (_selectedSlot == new EquipmentSlot(EquipmentSlotType.BothHands))
                {
                    console.WriteLine(
                        $"{new StyledText("LR", Styles.Player).Text}. {item.Name}");
                    console.WriteLine(item.Description);
                    foreach (var hint in _inputHandler.GetHints().Where(h => (h.Location & UiLocation.Inventory) != 0))
                        console.WriteHintLine(hint);
                }
                else
                {
                    console.WriteLine(
                        $"{new StyledText(new StyledText("LR", Styles.Player), Style.Faint).Text}. {item.Name}");
                }
            }
            else
            {
                console.Write($"{new StyledText(new StyledText("L", Styles.Player), Style.Faint).Text}. ");
                console.WriteLine(_player.Inventory.Equipment.LeftHand != null
                    ? _player.Inventory.Equipment.LeftHand.Name
                    : "Empty");
                console.Write($"{new StyledText(new StyledText("R", Styles.Player), Style.Faint).Text}. ");
                console.WriteLine(_player.Inventory.Equipment.RightHand != null
                    ? _player.Inventory.Equipment.RightHand.Name
                    : "Empty");
            }
        }

        if (!_player.Inventory.Backpack.IsEmpty)
            console.WriteLine();

        var i = 0;
        foreach (var item in _player.Inventory.Backpack.Items)
        {
            if (_selectedSlot == new BackpackSlot(i))
            {
                console.WriteLine($"{new StyledText((i + 1).ToString(), Styles.Player).Text}. {item.Name}");
                console.WriteLine(item.Description);
                foreach (var hint in _inputHandler.GetHints().Where(h => (h.Location & UiLocation.Backpack) != 0))
                    console.WriteHintLine(hint);
            }
            else
            {
                console.WriteLine(
                    $"{new StyledText(new StyledText((i + 1).ToString(), Style.Faint), Styles.Player).Text}. {item.Name}");
            }

            i++;
        }
    }

    private bool DrawActiveEffects()
    {
        if (_player.Effects.Count == 0) return false;
        var console = ConsoleHelper.GetInstance();
        console.WriteLine($"{new StyledText("Effects:", Style.Underline).Text}");
        foreach (var effect in _player.Effects.OrderByDescending(e => e.TurnsLeft))
        {
            console.WriteLine(effect.Name);
            console.WriteLine(effect.Description);
        }

        return true;
    }

    private bool DrawStandingOn()
    {
        if (CurrentItem == null) return false;
        var console = ConsoleHelper.GetInstance();

        console.WriteLine(new StyledText("Standing on:", Style.Underline));
        console.WriteLine(CurrentTile.Name);
        console.WriteLine(CurrentTile.Description);
        foreach (var hint in _inputHandler.GetHints().Where(h => (h.Location & UiLocation.StandingOn) != 0))
            console.WriteHintLine(hint);

        return true;
    }

    private bool DrawLookingAt()
    {
        if (LookingAt == null) return false;
        var console = ConsoleHelper.GetInstance();

        console.WriteLine(new StyledText("Looking at:", Style.Underline));
        console.WriteLine(LookingAt.Name);
        console.WriteLine(LookingAt.Description);
        return true;
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
        LookingAt = null;
        var newPos = _player.Position + direction;
        if (CanMoveTo(newPos))
        {
            _player.Position = newPos;
            return true;
        }

        if (IsInBounds(newPos))
        {
            LookingAt = this[newPos];
        }

        return false;
    }

    public Item? TryPickUpItem()
    {
        if (CurrentItem == null || _player.Inventory.Backpack.IsFull) return null;
        var item = CurrentItem;
        _player.PickUp(CurrentItem);
        CurrentTile.RemoveCurrentItem();
        return item;
    }

    private Item? TryDropItem(InventorySlot slot)
    {
        if (_player.Inventory[slot] == null) return null;

        var item = _player.Drop(slot);
        CurrentTile.Add(item);
        return item;
    }

    public Item? TryDropItem()
    {
        if (_selectedSlot == null) return null;
        var item = TryDropItem(_selectedSlot);
        _selectedSlot = null;
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
        if (!slot.IsValid(_player.Inventory)) return false;

        var normSlot = slot.Normalize(_player.Inventory, _selectedSlot);

        // deselect when selecting the current active slot
        if (_selectedSlot == normSlot)
        {
            _selectedSlot = null;
            return true;
        }

        if (_selectedSlot == null || _player.Inventory[_selectedSlot] == null)
        {
            _selectedSlot = normSlot;
            return true;
        }

        _player.Inventory.Swap(_selectedSlot, slot);
        slot = slot.Normalize(_player.Inventory, _selectedSlot);
        _selectedSlot = slot;

        return true;
    }

    public bool TryMoveToBackpack()
    {
        if (_selectedSlot == null || _player.Inventory[_selectedSlot] == null ||
            !_selectedSlot.CanMoveToBackpack || _player.Inventory.Backpack.IsFull) return false;
        _selectedSlot.MoveToBackpack(_player.Inventory);
        _selectedSlot = null;

        return true;
    }

    public Item? TryUseItem()
    {
        if (_selectedSlot == null) return null;
        if (_player.Inventory[_selectedSlot] is not IUsable item) return null;

        item.Use(_player);
        var res = _player.Drop(_selectedSlot);
        return res;
    }

    public void CycleItems(bool reverse) => CurrentTile.CycleItems(reverse);

    public bool IsInBounds(Position p) => p.X >= 0 && p.X < Width && p.Y >= 0 && p.Y < Height;

    private bool CanMoveTo(Position p) => IsInBounds(p) && this[p].IsPassable;

    public IEnumerator<Tile> GetEnumerator()
    {
        for (var y = 0; y < Height; y++)
        {
            for (var x = 0; x < Width; x++)
            {
                yield return _tiles[y, x];
            }
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}