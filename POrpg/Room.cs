using System.Diagnostics;
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
        _player = new Player(_playerInitialPosition);

        for (var y = 0; y < _height; y++)
        {
            for (var x = 0; x < _width; x++)
            {
                this[(x, y)] = x * y % 3 == 1 ? new WallTile() : new EmptyTile();
            }
        }

        this[(3, 3)] = new Sword();
        this[(3, 4)] = new Unlucky(new Unlucky(new Sword()));
        this[(3, 5)] = new Powerful(new Sword());
        this[(3, 6)] = new Unlucky(new Powerful(new Sword()));
    }

    public void Draw()
    {
        for (var y = 0; y < _height; y++)
        {
            for (var x = 0; x < _width; x++)
            {
                if ((x, y) == _player.Position)
                {
                    Console.Write(_player.Symbol);
                    continue;
                }

                Console.Write(this[(x, y)].Symbol);
            }

            Console.WriteLine();
        }

        var textMargin = _width + 5;
        var console = new ConsoleHelper(column: textMargin);
        console.Write("Player Stats:");
        foreach (var attribute in _player.Attributes)
        {
            console.Write($"{attribute.Key,-15} {attribute.Value}");
        }

        console.HorizontalDivider();
        var current = this[_player.Position];
        console.Write($"Standing on: {current.Name}");
        if (current.Description != null)
            console.Write($"{current.Description}");

        console.Column = 0;
        console.Line = _height + 1;
        console.Write("Move: WSAD/arrows");
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

    private bool CanMoveTo(Position p) =>
        p.X >= 0 && p.X < _width && p.Y >= 0 && p.Y < _height && this[p].IsPassable;
}