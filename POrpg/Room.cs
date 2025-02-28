using System.Diagnostics;

namespace POrpg;

public record struct Position(int X, int Y)
{
    public static implicit operator Position((int x, int y) p) => new() { X = p.x, Y = p.y };
}

public class Room
{
    private readonly Position _playerInitialPosition = (0, 0);

    private readonly IDrawable[,] _grid;
    private readonly Player _player;
    private readonly int _width;
    private readonly int _height;

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
    }

    public void Draw()
    {
        for (var y = 0; y < _height; y++)
        {
            for (var x = 0; x < _width; x++)
            {
                if ((x, y) == _player.Position)
                {
                    Console.Write(_player);
                    continue;
                }

                Console.Write(this[(x, y)]);
            }

            Console.WriteLine();
        }

        var textMargin = _width + 5;
        var line = 0;
        Console.SetCursorPosition(textMargin, line++);
        Console.Write("Player Stats:");
        Console.SetCursorPosition(textMargin, line++);
        Console.Write($"P: {_player.Strength}, A: {_player.Dexterity}, " +
                          $"H: {_player.Health}, L: {_player.Luck}, " +
                          $"A: {_player.Aggression}, W: {_player.Wisdom}");
        Console.SetCursorPosition(textMargin, line++);
        Console.Write($"Standing on: {this[_player.Position].Description}");
        
        Console.SetCursorPosition(0, _height + 1);
        Console.WriteLine("Move: WSAD/arrows");
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
            // TODO ?
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