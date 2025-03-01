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

        console.WriteLine(new StyledText("Player Stats:", Style.Underline));
        foreach (var attribute in _player.Attributes)
        {
            console.Write($"{attribute.Key,-15} ");
            console.WriteLine(new StyledText(attribute.Value.ToString(), Style.Gradient));
        }

        console.WriteLine();
        console.WriteLine($"{"Coins",-15} {new StyledText(_player.Coins.ToString(), Style.Yellow).Text}");
        console.WriteLine($"{"Gold",-15} {new StyledText(_player.Gold.ToString(), Style.Yellow).Text}");

        console.HorizontalDivider();
        if (CurrentItem != null)
        {
            console.WriteLine(new StyledText("Standing on:", Style.Underline));
            console.WriteLine(CurrentItem.Name);
            if (CurrentItem.Description != null)
                console.WriteLine($"{CurrentItem.Description}");
            console.WriteLine(new StyledText("(E) to pick up", Style.Faint));
        }

        console.Column = 0;
        console.Line = _height + 1;
        console.WriteLine(new StyledText("Move: WSAD/arrows", Style.Faint));
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

    public void ProcessInput(ConsoleKey input)
    {
        switch (input)
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
        }
    }

    private IItem? CurrentItem => _items[_player.Position.X, _player.Position.Y];

    private void RemoveCurrentItem() => _items[_player.Position.X, _player.Position.Y] = null;

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

    private void TryPickUpItem()
    {
        CurrentItem?.PickUp(_player);
        RemoveCurrentItem();
    }

    private bool CanMoveTo(Position p) =>
        p.X >= 0 && p.X < _width && p.Y >= 0 && p.Y < _height && this[p].IsPassable;
}