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

        /* Stats */
        console.WriteLine(new StyledText("Player Stats:", Style.Underline));
        foreach (var attribute in _player.Attributes)
        {
            console.Write($"{attribute.Key,-15} ");
            console.WriteLine(new StyledText(attribute.Value.ToString(), Style.Gradient));
        }

        console.WriteLine();
        console.WriteLine($"{"Coins",-15} {new StyledText(_player.Coins.ToString(), Style.Yellow).Text}");
        console.WriteLine($"{"Gold",-15} {new StyledText(_player.Gold.ToString(), Style.Yellow).Text}");

        /* Inventory */
        if (_player.Inventory.Count > 0)
        {
            console.HorizontalDivider();
            console.WriteLine(new StyledText("Inventory:", Style.Underline));
        }

        var i = 1;
        foreach (var item in _player.Inventory)
        {
            if (i == _selectedItem)
            {
                console.WriteLine($"{new StyledText(i.ToString(), Style.Magenta).Text}. {item.Name}");
                var desc = _player.Inventory.ElementAt(i - 1).Description;
                if (desc != null)
                    console.WriteLine(desc);
                console.WriteLine(InputHint("Q", " to drop"));
            }
            else
            {
                console.WriteLine($"{InputHint(i.ToString())}. {item.Name}");
            }

            i++;
        }

        /* Standing on */
        console.HorizontalDivider();
        if (CurrentItem != null)
        {
            console.WriteLine(new StyledText("Standing on:", Style.Underline));
            console.WriteLine(CurrentItem.Name);
            if (CurrentItem.Description != null)
                console.WriteLine($"{CurrentItem.Description}");
            console.WriteLine(InputHint("E", " to pick up"));
        }

        console.Column = 0;
        console.Line = _height + 1;
        console.WriteLine(InputHint("WSAD", " to move"));
        sw.Stop();
        console.WriteLine(new StyledText($"Frame time: {sw.Elapsed.Microseconds} μs", Style.Faint));
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
            case ConsoleKey.D1 or ConsoleKey.D2 or ConsoleKey.D3 or ConsoleKey.D4 or ConsoleKey.D5 or ConsoleKey.D6
                or ConsoleKey.D7 or ConsoleKey.D8 or ConsoleKey.D9:
                TrySelectItem(int.Parse(input.KeyChar.ToString()));
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

    private int? _selectedItem;

    private void RemoveCurrentItem() => _items[_player.Position.X, _player.Position.Y] = null;

    private static string InputHint(string keys, string description = "") =>
        new StyledText($"{new StyledText(keys, Style.Magenta).Text}{description}", Style.Faint).Text;

    private void TryPickUpItem()
    {
        if (CurrentItem == null) return;
        _player.PickUp(CurrentItem);
        RemoveCurrentItem();
    }

    private void TryDropItem()
    {
        if (_selectedItem == null || _selectedItem > _player.Inventory.Count) return;
        // TODO: cannot drop on another item?
        if (CurrentItem != null) return;
        CurrentItem = _player.Drop(_selectedItem.Value - 1);
        _selectedItem = null;
    }

    private void TrySelectItem(int index)
    {
        if (index > _player.Inventory.Count) return;
        if (index == _selectedItem) _selectedItem = null;
        else _selectedItem = index;
    }

    private bool CanMoveTo(Position p) =>
        p.X >= 0 && p.X < _width && p.Y >= 0 && p.Y < _height && this[p].IsPassable;
}