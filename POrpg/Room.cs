using System.Diagnostics;
using POrpg.ConsoleHelpers;
using POrpg.Items;
using POrpg.Items.Effects;

namespace POrpg;

public record struct Position(int X, int Y)
{
    public static implicit operator Position((int x, int y) p) => new() { X = p.x, Y = p.y };
}

public class Room
{
    private readonly Position _playerInitialPosition = (0, 0);

    private readonly int _width;
    private readonly int _height;
    private readonly IDrawable[,] _grid;
    private readonly IItem?[,] _items;

    // TODO
    private readonly Player _player;

    public IDrawable this[Position p]
    {
        get => _grid[p.X, p.Y];
        set => _grid[p.X, p.Y] = value;
    }

    public Room(int width, int height)
    {
        (_width, _height) = (width, height);
        _grid = new IDrawable[width, height];
        _items = new IItem?[width, height];
        _player = new Player(_playerInitialPosition);

        for (var y = 0; y < _height; y++)
        {
            for (var x = 0; x < _width; x++)
            {
                this[(x, y)] = x * y % 3 == 1 ? new WallTile() : new EmptyTile();
            }
        }

        _items[3, 3] = new Sword();
        _items[3, 4] = new Unlucky(new Unlucky(new Sword()));
        _items[3, 5] = new Powerful(new Sword());
        _items[3, 6] = new Unlucky(new Powerful(new Sword()));
        _items[5, 3] = new Coin();
        _items[6, 3] = new Coin();
        _items[7, 3] = new Gold();
        _items[8, 3] = new Gold();
    }

    public void Draw()
    {
        var sw = Stopwatch.StartNew();
        var console = new ConsoleHelper();

        for (var y = 0; y < _height; y++)
        {
            for (var x = 0; x < _width; x++)
            {
                if ((x, y) == _player.Position)
                {
                    console.Write(new StyledText(_player.Symbol, Style.Magenta));
                    continue;
                }

                var item = _items[x, y];
                console.Write(item != null ? item.Symbol : this[(x, y)].Symbol);
            }

            console.WriteLine();
        }

        console.Column = _width + 5;
        console.Line = 1;

        DrawStats(console);
        DrawInventory(console);
        DrawStandingOn(console);

        console.Column = 0;
        console.Line = _height + 1;
        console.WriteLine(InputHint("WSAD", "Move"));
        sw.Stop();
        console.WriteLine(new StyledText($"Frame time: {sw.Elapsed.Milliseconds} ms", Style.Faint));
    }

    private void DrawStats(ConsoleHelper console)
    {
        console.WriteLine(new StyledText("Player Stats:", Style.Underline));
        foreach (var attribute in _player.Attributes)
        {
            console.Write($"{attribute.Key,-15} ");
            console.WriteLine(new StyledText(attribute.Value.ToString(), Style.Gradient));
        }

        console.WriteLine();
        console.WriteLine($"{"Coins",-15} {new StyledText(_player.Coins.ToString(), Style.Yellow).Text}");
        console.WriteLine($"{"Gold",-15} {new StyledText(_player.Gold.ToString(), Style.Yellow).Text}");
    }

    private void DrawInventory(ConsoleHelper console)
    {
        console.HorizontalDivider();
        console.WriteLine(new StyledText("Inventory:", Style.Underline));

        if (_selectedSlot is InventorySlot.HandSlot(Hand.Left))
        {
            console.Write($"{new StyledText("L", Style.Magenta).Text}. ");
            if (_player.Inventory.LeftHand != null)
            {
                console.WriteLine(_player.Inventory.LeftHand.Name);
                console.WriteLine(_player.Inventory.LeftHand.Description);
                console.WriteLine(InputHint("Q", "Drop"));
                console.WriteLine(InputHint("B", "Move to backpack"));
            }
            else
                console.WriteLine("Empty");
        }
        else
        {
            console.Write($"{InputHint("L")}. ");
            console.WriteLine(_player.Inventory.LeftHand != null ? _player.Inventory.LeftHand.Name : "Empty");
        }

        if (_selectedSlot is InventorySlot.HandSlot(Hand.Right))
        {
            console.Write($"{new StyledText("R", Style.Magenta).Text}. ");
            if (_player.Inventory.RightHand != null)
            {
                console.WriteLine(_player.Inventory.RightHand.Name);
                console.WriteLine(_player.Inventory.RightHand.Description);
                console.WriteLine(InputHint("Q", "Drop"));
                console.WriteLine(InputHint("B", "Move to backpack"));
            }
            else
                console.WriteLine("Empty");
        }
        else
        {
            console.Write($"{InputHint("R")}. ");
            console.WriteLine(_player.Inventory.RightHand != null ? _player.Inventory.RightHand.Name : "Empty");
        }

        if (_player.Inventory.Backpack.Count > 0)
            console.WriteLine();

        var i = 0;
        foreach (var item in _player.Inventory.Backpack)
        {
            if (_selectedSlot is InventorySlot.BackpackSlot s && i == s.Slot)
            {
                console.WriteLine($"{new StyledText((i + 1).ToString(), Style.Magenta).Text}. {item.Name}");
                console.WriteLine(_player.Inventory[new InventorySlot.BackpackSlot(i)]!.Description);
                console.WriteLine(InputHint("Q", "Drop"));
            }
            else
            {
                console.WriteLine($"{InputHint((i + 1).ToString())}. {item.Name}");
            }

            i++;
        }
    }

    private void DrawStandingOn(ConsoleHelper console)
    {
        console.HorizontalDivider();
        if (CurrentItem == null) return;

        console.WriteLine(new StyledText("Standing on:", Style.Underline));
        console.WriteLine(CurrentItem.Name);
        console.WriteLine(CurrentItem.Description);
        console.WriteLine(InputHint("E", "Pick up"));
    }

    private enum Direction
    {
        Up,
        Down,
        Left,
        Right
    }

    public void ProcessInput(ConsoleKeyInfo input)
    {
        switch (input.Key)
        {
            case ConsoleKey.W or ConsoleKey.UpArrow:
                TryMovePlayer(Direction.Up);
                break;
            case ConsoleKey.S or ConsoleKey.DownArrow:
                TryMovePlayer(Direction.Down);
                break;
            case ConsoleKey.A or ConsoleKey.LeftArrow:
                TryMovePlayer(Direction.Left);
                break;
            case ConsoleKey.D or ConsoleKey.RightArrow:
                TryMovePlayer(Direction.Right);
                break;
            case ConsoleKey.E:
                TryPickUpItem();
                break;
            case ConsoleKey.Q:
                TryDropItem();
                break;
            case ConsoleKey.L:
                TrySelectItem(new InventorySlot.HandSlot(Hand.Left));
                break;
            case ConsoleKey.R:
                TrySelectItem(new InventorySlot.HandSlot(Hand.Right));
                break;
            case ConsoleKey.B:
                TryMoveToBackpack();
                break;
            case ConsoleKey.D1 or ConsoleKey.D2 or ConsoleKey.D3 or ConsoleKey.D4 or ConsoleKey.D5 or ConsoleKey.D6
                or ConsoleKey.D7 or ConsoleKey.D8 or ConsoleKey.D9:
                TrySelectItem(new InventorySlot.BackpackSlot(int.Parse(input.KeyChar.ToString()) - 1));
                break;
        }
    }

    private void TryMovePlayer(Direction direction)
    {
        var newPos = direction switch
        {
            Direction.Up => (_player.Position.X, _player.Position.Y - 1),
            Direction.Down => (_player.Position.X, _player.Position.Y + 1),
            Direction.Left => (_player.Position.X - 1, _player.Position.Y),
            Direction.Right => (_player.Position.X + 1, _player.Position.Y),
            // TODO
            _ => throw new UnreachableException()
        };
        if (CanMoveTo(newPos))
        {
            _player.Position = newPos;
        }
    }

    private IItem? CurrentItem
    {
        get => _items[_player.Position.X, _player.Position.Y];
        set => _items[_player.Position.X, _player.Position.Y] = value;
    }

    private InventorySlot? _selectedSlot;

    private void RemoveCurrentItem() => _items[_player.Position.X, _player.Position.Y] = null;

    private static string InputHint(string keys, string? description = null)
    {
        if (description == null)
            return new StyledText(new StyledText(keys, Style.Magenta), Style.Faint).Text;
        
        return new StyledText($"{description} ({new StyledText(keys, Style.Magenta).Text})", Style.Faint).Text;
    }

    private void TryPickUpItem()
    {
        if (CurrentItem == null) return;
        _player.PickUp(CurrentItem);
        RemoveCurrentItem();
    }

    private void TryDropItem()
    {
        if (_selectedSlot == null || _player.Inventory[_selectedSlot] == null) return;
        // TODO: cannot drop on another item?
        if (CurrentItem != null) return;
        CurrentItem = _player.Drop(_selectedSlot);
        _selectedSlot = null;
    }

    private void TrySelectItem(InventorySlot slot)
    {
        if (slot is InventorySlot.BackpackSlot s && s.Slot >= _player.Inventory.Backpack.Count)
            return;
        if (_selectedSlot == slot)
        {
            _selectedSlot = null;
            return;
        }

        if (_selectedSlot == null || _player.Inventory[_selectedSlot] == null)
        {
            _selectedSlot = slot;
            return;
        }

        _player.Inventory.Swap(slot, _selectedSlot);
        _selectedSlot = slot;
    }

    private void TryMoveToBackpack()
    {
        if (_selectedSlot == null || _player.Inventory[_selectedSlot] == null) return;
        if (_selectedSlot is not InventorySlot.HandSlot slot) return;
        _player.Inventory.MoveToBackpack(slot);
        _selectedSlot = null;
    }

    private bool CanMoveTo(Position p) =>
        p.X >= 0 && p.X < _width && p.Y >= 0 && p.Y < _height && this[p].IsPassable;
}