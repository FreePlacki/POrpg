using POrpg.ConsoleHelpers;
using POrpg.Items;

namespace POrpg;

public abstract class Tile : IDrawable
{
    public abstract string Symbol { get; }
    public abstract string Name { get; }
    public virtual bool IsPassable => true;

    public virtual IEnumerable<IItem> Items => [];
    public virtual void Add(IItem item) => throw new InvalidOperationException();
    public virtual IItem? CurrentItem => throw new InvalidOperationException();
    public virtual void RemoveCurrentItem() => throw new InvalidOperationException();
    public virtual bool HasManyItems => throw new InvalidOperationException();
    public virtual void CycleItems(bool reverse = false) => throw new InvalidOperationException();
}

public class FloorTile : Tile
{
    private readonly List<IItem> _items;
    private bool IsEmpty => _items.Count == 0;

    private int _currentItemIndex;
    public override bool HasManyItems => _items.Count > 1;
    public override IItem? CurrentItem => _items.ElementAtOrDefault(_currentItemIndex);
    public override IEnumerable<IItem> Items => _items;

    public override string Symbol => IsEmpty ? " " :
        HasManyItems ? new StyledText(CurrentItem!.Symbol, Style.Underline).Text : CurrentItem!.Symbol;

    public override string Name => IsEmpty ? "Empty Tile" : _items[0].Name;

    public FloorTile(params IItem[] items)
    {
        _items = items.ToList();
        _currentItemIndex = items.Length - 1;
    }

    public override void CycleItems(bool reverse = false)
    {
        var offset = reverse ? -1 : 1;
        _currentItemIndex = (_currentItemIndex + offset + _items.Count) % _items.Count;
    }

    public override void Add(IItem item)
    {
        _items.Add(item);
        _currentItemIndex++;
    }

    public override void RemoveCurrentItem()
    {
        _items.RemoveAt(_currentItemIndex);
        if (IsEmpty) return;
        CycleItems(reverse: true);
    }
}

public class WallTile : Tile
{
    public override string Symbol => "\u2588";
    public override string Name => "Wall";
    public override bool IsPassable => false;
}