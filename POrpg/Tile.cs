using POrpg.ConsoleUtils;
using POrpg.Enemies;
using POrpg.Items;

namespace POrpg;

public abstract class Tile : IDrawable
{
    public abstract string Symbol { get; }
    public abstract string Name { get; }
    public virtual string? Description => null;
    public abstract bool IsPassable { get; }
    public Enemy? Enemy { get; set; }

    public virtual void Add(Item item) => throw new InvalidOperationException();
    public virtual void Add(Enemy enemy) => throw new InvalidOperationException();
    public virtual Item? CurrentItem => null;
    public virtual void RemoveCurrentItem() => throw new InvalidOperationException();
    public virtual bool HasManyItems => false;
    public virtual void CycleItems(bool reverse = false) => throw new InvalidOperationException();
}

public class FloorTile : Tile
{
    public override bool IsPassable => Enemy == null;
    public override bool HasManyItems => Items.Count > 1;
    public override Item? CurrentItem => Items.ElementAtOrDefault(_currentItemIndex);
    public List<Item> Items { get; }

    private bool IsEmpty => Items.Count == 0;
    private int _currentItemIndex;

    public override string Symbol =>
        Enemy != null ? !IsEmpty ? new StyledText(Enemy.Symbol, Styles.Stacked).ToString() : Enemy.Symbol :
        IsEmpty ? " " :
        HasManyItems ? new StyledText(CurrentItem!.Symbol, Styles.Stacked).ToString() : CurrentItem!.Symbol;

    public override string Name =>
        Enemy?.Name ?? CurrentItem?.Name ?? "Empty Tile";

    public override string? Description =>
        Enemy?.Description ?? CurrentItem?.Description ?? null;

    public FloorTile(List<Item>? items = null)
    {
        Items = items ?? [];
        _currentItemIndex = Items.Count - 1;
    }

    public override void CycleItems(bool reverse = false)
    {
        if (!HasManyItems) return;
        var offset = reverse ? -1 : 1;
        _currentItemIndex = (_currentItemIndex + offset + Items.Count) % Items.Count;
    }

    public override void Add(Item item)
    {
        Items.Add(item);
        _currentItemIndex = Items.Count - 1;
    }

    public override void Add(Enemy enemy)
    {
        Enemy = enemy;
    }

    public override void RemoveCurrentItem()
    {
        Items.RemoveAt(_currentItemIndex);
        if (IsEmpty) return;
        _currentItemIndex = (_currentItemIndex - 1 + Items.Count) % Items.Count;
    }
}

public class WallTile : Tile
{
    public override string Symbol => new StyledText("\u2588", Style.Faint).ToString();
    public override string Name => "Wall";
    public override bool IsPassable => false;
}