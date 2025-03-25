using System.Collections;
using System.Diagnostics;
using POrpg.Commands;
using POrpg.ConsoleHelpers;
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
    private readonly Tile[,] _tiles;

    private readonly Player _player;

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

    public void Draw()
    {
        var sw = Stopwatch.StartNew();
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
        DrawStats();
        console.HorizontalDivider();
        DrawInventory();
        console.HorizontalDivider();
        console.ChangeColumn(2);
        if (DrawStandingOn())
            console.HorizontalDivider();
        if (DrawLookingAt())
            console.HorizontalDivider();

        console.ChangeColumn(0);
        console.WriteLine();
        if (_lastCommand?.Description != null)
            console.WriteLine(_lastCommand.Description);
        console.WriteLine($"{ConsoleHelper.InputHint("WSAD", "Move")}  " +
                          $"{ConsoleHelper.InputHint("C", "Redraw")}  " +
                          $"{ConsoleHelper.InputHint("?", "Help")}");
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
                console.WriteLine(ConsoleHelper.InputHint("Q", "Drop"));
                console.WriteLine(
                    ConsoleHelper.InputHint("B",
                        $"Move to backpack{(_player.Inventory.Backpack.IsFull ? "[full]" : "")}"));
            }
            else
            {
                console.WriteLine("Empty");
            }

            console.Write($"{ConsoleHelper.InputHint("R")}. ");
            var rightHand = _player.Inventory.Equipment.RightHand;
            console.WriteLine(rightHand != null ? rightHand.Name : "Empty");
        }
        else if (_selectedSlot == new EquipmentSlot(EquipmentSlotType.RightHand))
        {
            console.Write($"{ConsoleHelper.InputHint("L")}. ");
            var leftHand = _player.Inventory.Equipment.LeftHand;
            console.WriteLine(leftHand != null ? leftHand.Name : "Empty");

            console.Write($"{new StyledText("R", Styles.Player).Text}. ");
            var rightHand = _player.Inventory.Equipment.RightHand;
            if (rightHand != null)
            {
                console.WriteLine(rightHand.Name);
                console.WriteLine(rightHand.Description);
                console.WriteLine(ConsoleHelper.InputHint("Q", "Drop"));
                console.WriteLine(
                    ConsoleHelper.InputHint("B",
                        $"Move to backpack{(_player.Inventory.Backpack.IsFull ? "[full]" : "")}"));
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
                    console.WriteLine(ConsoleHelper.InputHint("Q", "Drop"));
                    console.WriteLine(
                        ConsoleHelper.InputHint("B",
                            $"Move to backpack{(_player.Inventory.Backpack.IsFull ? "[full]" : "")}"));
                }
                else
                {
                    console.WriteLine($"{ConsoleHelper.InputHint("LR")}. {item.Name}");
                }
            }
            else
            {
                console.Write($"{ConsoleHelper.InputHint("L")}. ");
                console.WriteLine(_player.Inventory.Equipment.LeftHand != null
                    ? _player.Inventory.Equipment.LeftHand.Name
                    : "Empty");
                console.Write($"{ConsoleHelper.InputHint("R")}. ");
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
                console.WriteLine(ConsoleHelper.InputHint("Q", "Drop"));
            }
            else
            {
                console.WriteLine($"{ConsoleHelper.InputHint((i + 1).ToString())}. {item.Name}");
            }

            i++;
        }
    }

    private bool DrawStandingOn()
    {
        if (CurrentItem == null) return false;
        var console = ConsoleHelper.GetInstance();

        console.WriteLine(new StyledText("Standing on:", Style.Underline));
        console.WriteLine(CurrentTile.Name);
        console.WriteLine(CurrentTile.Description);
        console.WriteLine(ConsoleHelper.InputHint("E",
            $"Pick up{(_player.Inventory.Backpack.IsFull ? "[backpack full]" : "")}"));
        if (CurrentTile.HasManyItems)
            console.WriteLine(ConsoleHelper.InputHint(",.", "Cycle items"));

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

    public void ProcessInput(ConsoleKeyInfo input)
    {
        switch (input.Key)
        {
            case ConsoleKey.W or ConsoleKey.UpArrow:
                _lastCommand = new MovePlayerCommand(this, (0, -1));
                break;
            case ConsoleKey.S or ConsoleKey.DownArrow:
                _lastCommand = new MovePlayerCommand(this, (0, 1));
                break;
            case ConsoleKey.A or ConsoleKey.LeftArrow:
                _lastCommand = new MovePlayerCommand(this, (-1, 0));
                break;
            case ConsoleKey.D or ConsoleKey.RightArrow:
                _lastCommand = new MovePlayerCommand(this, (1, 0));
                break;
            case ConsoleKey.E:
                _lastCommand = new PickUpItemCommand(this);
                break;
            case ConsoleKey.Q:
                _lastCommand = new DropItemCommand(this);
                break;
            case ConsoleKey.L:
                _lastCommand = new SelectItemCommand(this, new EquipmentSlot(EquipmentSlotType.LeftHand));
                break;
            case ConsoleKey.R:
                _lastCommand = new SelectItemCommand(this, new EquipmentSlot(EquipmentSlotType.RightHand));
                break;
            case ConsoleKey.B:
                _lastCommand = new MoveToBackpackCommand(this);
                break;
            case ConsoleKey.OemPeriod:
                _lastCommand = new CycleItemsCommand(this);
                break;
            case ConsoleKey.OemComma:
                _lastCommand = new CycleItemsCommand(this, reverse: true);
                break;
            case ConsoleKey.D1 or ConsoleKey.D2 or ConsoleKey.D3 or ConsoleKey.D4 or ConsoleKey.D5 or ConsoleKey.D6
                or ConsoleKey.D7 or ConsoleKey.D8 or ConsoleKey.D9:
                if (int.TryParse(input.KeyChar.ToString(), out var slot))
                    _lastCommand = new SelectItemCommand(this, new BackpackSlot(slot - 1));

                break;
        }
        
        _lastCommand?.Execute();
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

    private Tile CurrentTile => this[_player.Position];
    private Item? CurrentItem => CurrentTile.CurrentItem;
    private Tile? LookingAt { get; set; }
    private ICommand? _lastCommand;

    private InventorySlot? _selectedSlot;

    public Item? TryPickUpItem()
    {
        if (CurrentItem == null || _player.Inventory.Backpack.IsFull) return null;
        var item = CurrentItem;
        _player.PickUp(CurrentItem);
        CurrentTile.RemoveCurrentItem();
        return item;
    }

    public Item? TryDropItem()
    {
        if (_selectedSlot == null || _player.Inventory[_selectedSlot] == null) return null;
        var item = _player.Drop(_selectedSlot);
        CurrentTile.Add(item);
        _selectedSlot = null;
        return item;
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