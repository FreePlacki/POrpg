using POrpg.ConsoleHelpers;
using POrpg.Enemies;
using POrpg.Items;

namespace POrpg;

public abstract class Tile : IDrawable
{
    public abstract string Symbol { get; }
    public abstract string Name { get; }
    public virtual string? Description => null;
    public abstract bool IsPassable { get; }

    public virtual IEnumerable<Item> Items => [];
    public virtual void Add(Item item) => throw new InvalidOperationException();
    public virtual void Add(Enemy enemy) => throw new InvalidOperationException();
    public virtual Item? CurrentItem => throw new InvalidOperationException();
    public virtual void RemoveCurrentItem() => throw new InvalidOperationException();
    public virtual bool HasManyItems => throw new InvalidOperationException();
    public virtual void CycleItems(bool reverse = false) => throw new InvalidOperationException();
}

public class FloorTile : Tile
{
    private readonly List<Item> _items;
    private Enemy? _enemy;
    private bool IsEmpty => _items.Count == 0;

    private int _currentItemIndex;
    public override bool IsPassable => _enemy == null;
    public override bool HasManyItems => _items.Count > 1;
    public override Item? CurrentItem => _items.ElementAtOrDefault(_currentItemIndex);
    public override IEnumerable<Item> Items => _items;

    public override string Symbol =>
        _enemy != null ? !IsEmpty ? new StyledText(_enemy.Symbol, Styles.Stacked).Text : _enemy.Symbol :
        IsEmpty ? " " :
        HasManyItems ? new StyledText(CurrentItem!.Symbol, Styles.Stacked).Text : CurrentItem!.Symbol;

    public override string Name =>
        _enemy?.Name ?? CurrentItem?.Name ?? "Empty Tile";

    public override string? Description =>
        _enemy?.Description ?? CurrentItem?.Description ?? null;


    public FloorTile(params Item[] items)
    {
        _items = items.ToList();
        _currentItemIndex = items.Length - 1;
    }

    public override void CycleItems(bool reverse = false)
    {
        if (!HasManyItems) return;
        var offset = reverse ? -1 : 1;
        _currentItemIndex = (_currentItemIndex + offset + _items.Count) % _items.Count;
    }

    public override void Add(Item item)
    {
        _items.Add(item);
        _currentItemIndex = _items.Count - 1;
    }

    public override void Add(Enemy enemy)
    {
        _enemy = enemy;
    }

    public override void RemoveCurrentItem()
    {
        _items.RemoveAt(_currentItemIndex);
        if (IsEmpty) return;
        _currentItemIndex = (_currentItemIndex - 1 + _items.Count) % _items.Count;
    }
}

public class WallTile : Tile
{
    public override string Symbol => new StyledText("\u2588", Style.Faint).Text;
    public override string Name => "Wall";
    public override bool IsPassable => false;
}