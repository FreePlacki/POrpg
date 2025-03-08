using System.Diagnostics;
using POrpg.ConsoleHelpers;
using POrpg.Inventory;
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
    private readonly Tile[,] _tiles;

    private readonly Player _player;

    private Tile this[Position p]
    {
        get => _tiles[p.X, p.Y];
        set => _tiles[p.X, p.Y] = value;
    }

    public Room(int width, int height)
    {
        (_width, _height) = (width, height);
        _tiles = new Tile[width, height];
        _player = new Player(_playerInitialPosition);

        for (var y = 0; y < _height; y++)
        {
            for (var x = 0; x < _width; x++)
            {
                this[(x, y)] = x * y % 3 == 1 ? new WallTile() : new FloorTile();
            }
        }

        _tiles[3, 3] = new FloorTile(new Sword());
        _tiles[3, 4] = new FloorTile(new Unlucky(new Unlucky(new Sword())));
        _tiles[3, 5] = new FloorTile(new Powerful(new Sword()));
        _tiles[3, 6] = new FloorTile(new Unlucky(new Powerful(new Sword())));
        _tiles[3, 7] = new FloorTile(new Unlucky(new TwoHanded(new Powerful(new Sword()))));
        _tiles[3, 9] = new FloorTile(new Legendary(new Unlucky(new Sword())));
        _tiles[3, 10] = new FloorTile(new Powerful(new Powerful(new Powerful(new Bow()))));
        _tiles[5, 3] = new FloorTile(new Coin());
        _tiles[6, 3] = new FloorTile(new Coin(), new Gold());
        _tiles[7, 3] = new FloorTile(new Gold());
        _tiles[8, 3] = new FloorTile(new Gold());

        _tiles[10, 3] = new FloorTile(new UnusableItem("Apple"));
        _tiles[11, 3] = new FloorTile(new UnusableItem("Rock"));
        _tiles[12, 3] = new FloorTile(new UnusableItem("Broken Sword"));
    }

    public void Draw(ConsoleHelper console)
    {
        var sw = Stopwatch.StartNew();

        for (var y = 0; y < _height; y++)
        {
            for (var x = 0; x < _width; x++)
            {
                if ((x, y) == _player.Position)
                {
                    console.Write(new StyledText(_player.Symbol, Style.Magenta));
                    continue;
                }

                console.Write(_tiles[x, y].Symbol);
            }

            console.WriteLine();
        }

        console.ChangeColumn(1);

        DrawStats(console);
        console.HorizontalDivider();
        DrawInventory(console);
        console.HorizontalDivider();
        DrawStandingOn(console);

        console.ChangeColumn(0);
        console.WriteLine();
        console.WriteLine($"{InputHint("WSAD", "Move")}\t{InputHint("C", "Redraw")}");
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
        console.WriteLine(new StyledText("Inventory:", Style.Underline));

        if (_selectedSlot == new EquipmentSlot(EquipmentSlotType.LeftHand))
        {
            console.Write($"{new StyledText("L", Style.Magenta).Text}. ");
            var leftHand = _player.Inventory[new EquipmentSlot(EquipmentSlotType.LeftHand)];
            if (leftHand != null)
            {
                console.WriteLine(leftHand.Name);
                console.WriteLine(leftHand.Description);
                console.WriteLine(InputHint("Q", "Drop"));
                console.WriteLine(InputHint("B", "Move to backpack"));
            }
            else
            {
                console.WriteLine("Empty");
            }

            console.Write($"{InputHint("R")}. ");
            var rightHand = _player.Inventory.Equipment.RightHand;
            console.WriteLine(rightHand != null ? rightHand.Name : "Empty");
        }
        else if (_selectedSlot == new EquipmentSlot(EquipmentSlotType.RightHand))
        {
            console.Write($"{InputHint("L")}. ");
            var leftHand = _player.Inventory.Equipment.LeftHand;
            console.WriteLine(leftHand != null ? leftHand.Name : "Empty");

            console.Write($"{new StyledText("R", Style.Magenta).Text}. ");
            var rightHand = _player.Inventory.Equipment.RightHand;
            if (rightHand != null)
            {
                console.WriteLine(rightHand.Name);
                console.WriteLine(rightHand.Description);
                console.WriteLine(InputHint("Q", "Drop"));
                console.WriteLine(InputHint("B", "Move to backpack"));
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
                        $"{new StyledText("LR", Style.Magenta).Text}. {item.Name}");
                    console.WriteLine(item.Description);
                    console.WriteLine(InputHint("Q", "Drop"));
                    console.WriteLine(InputHint("B", "Move to backpack"));
                }
                else
                {
                    console.WriteLine($"{InputHint("LR")}. {item.Name}");
                }
            }
            else
            {
                console.Write($"{InputHint("L")}. ");
                console.WriteLine(_player.Inventory.Equipment.LeftHand != null
                    ? _player.Inventory.Equipment.LeftHand.Name
                    : "Empty");
                console.Write($"{InputHint("R")}. ");
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
                console.WriteLine($"{new StyledText((i + 1).ToString(), Style.Magenta).Text}. {item.Name}");
                console.WriteLine(item.Description);
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
        if (CurrentItem == null) return;

        console.WriteLine(new StyledText("Standing on:", Style.Underline));
        console.WriteLine(CurrentItem.Name);
        console.WriteLine(CurrentItem.Description);
        console.WriteLine(InputHint("E", "Pick up"));
        if (CurrentTile.HasManyItems)
        {
            console.WriteLine(InputHint(",.", "Cycle items"));
        }
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
                TrySelectItem(new EquipmentSlot(EquipmentSlotType.LeftHand));
                break;
            case ConsoleKey.R:
                TrySelectItem(new EquipmentSlot(EquipmentSlotType.RightHand));
                break;
            case ConsoleKey.B:
                TryMoveToBackpack();
                break;
            case ConsoleKey.OemPeriod:
                CurrentTile.CycleItems();
                break;
            case ConsoleKey.OemComma:
                CurrentTile.CycleItems(reverse: true);
                break;
            case ConsoleKey.D1 or ConsoleKey.D2 or ConsoleKey.D3 or ConsoleKey.D4 or ConsoleKey.D5 or ConsoleKey.D6
                or ConsoleKey.D7 or ConsoleKey.D8 or ConsoleKey.D9:
                TrySelectItem(new BackpackSlot(int.Parse(input.KeyChar.ToString()) - 1));
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

    private Tile CurrentTile => _tiles[_player.Position.X, _player.Position.Y];
    private Item? CurrentItem => CurrentTile.CurrentItem;

    private InventorySlot? _selectedSlot;

    private void RemoveCurrentItem() => _tiles[_player.Position.X, _player.Position.Y].RemoveCurrentItem();

    private static string InputHint(string keys, string? description = null)
    {
        return description == null
            ? new StyledText(new StyledText(keys, Style.Magenta), Style.Faint).Text
            : new StyledText($"{description} ({new StyledText(keys, Style.Magenta).Text})", Style.Faint).Text;
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
        CurrentTile.Add(_player.Drop(_selectedSlot));
        _selectedSlot = null;
    }

    private void TrySelectItem(InventorySlot slot)
    {
        if (!slot.IsValid(_player.Inventory)) return;

        if (slot is EquipmentSlot &&
            (_selectedSlot?.Get(_player.Inventory)?.EquipmentSlotType == EquipmentSlotType.BothHands ||
             (_player.Inventory.Equipment.BothHands != null && _selectedSlot == null)))
            slot = new EquipmentSlot(EquipmentSlotType.BothHands);

        // deselect when selecting the current active slot
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

        _player.Inventory.Swap(_selectedSlot, slot);
        _selectedSlot = slot;
    }

    private void TryMoveToBackpack()
    {
        if (_selectedSlot == null || _player.Inventory[_selectedSlot] == null ||
            !_selectedSlot.CanMoveToBackpack) return;
        _selectedSlot.MoveToBackpack(_player.Inventory);
        _selectedSlot = null;
    }

    private bool CanMoveTo(Position p) =>
        p.X >= 0 && p.X < _width && p.Y >= 0 && p.Y < _height && this[p].IsPassable;
}