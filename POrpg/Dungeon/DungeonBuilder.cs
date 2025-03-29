using POrpg.Enemies;
using POrpg.Items;
using POrpg.Items.Effects;
using POrpg.Items.Effects.WeaponEffects;
using POrpg.Items.Potions;
using POrpg.Items.Weapons;

namespace POrpg.Dungeon;

public enum InitialDungeonState
{
    Empty,
    Filled,
}

public class DungeonBuilder : IDungeonBuilder<Dungeon>
{
    private readonly Dungeon _dungeon;
    private static readonly Random Rng = new();
    private readonly Position _playerInitialPosition;

    private readonly Func<Item>[] _itemConstructors =
    [
        () => new UnusableItem("Apple"),
        () => new UnusableItem("Rock"),
        () => new UnusableItem("Broken Sword")
    ];

    private readonly Func<Item>[] _moneyConstructors =
    [
        () => new Coin(), () => new Gold()
    ];

    private readonly Func<Weapon>[] _weaponConstructors =
    [
        () => new Sword(), () => new Bow(), () => new Dagger()
    ];

    private readonly Func<Potion>[] _potionConstructors =
    [
        () => new HealthPotion(), () => new StrengthPotion(Rng.Next(3, 10)), () => new LuckPotion(Rng.Next(5, 15))
    ];

    private readonly Func<Enemy>[] _enemyConstructors =
    [
        () => new Orc(), () => new Skeleton()
    ];

    private readonly Func<Item, Item>[] _itemEffectConstructors =
    [
        i => new Legendary(i), i => new Unlucky(i), i => new TwoHanded(i)
    ];

    private readonly Func<Weapon, Weapon>[] _weaponEffectConstructors =
    [
        w => new LegendaryWeapon(w), w => new UnluckyWeapon(w),
        w => new Powerful(w), w => new TwoHandedWeapon(w)
    ];

    public DungeonBuilder(InitialDungeonState initialState, int width, int height, Position playerInitialPosition)
    {
        _playerInitialPosition = playerInitialPosition;
        _dungeon = new Dungeon(initialState, width, height, playerInitialPosition);
    }

    private void AddFloor(Position position)
    {
        if (!_dungeon[position].IsPassable)
            _dungeon[position] = new FloorTile();
    }

    private DungeonBuilder AddRoom(Position position, int width, int height)
    {
        for (var y = position.Y; y < position.Y + height; y++)
        {
            for (var x = position.X; x < position.X + width; x++)
            {
                AddFloor((x, y));
            }
        }

        return this;
    }

    public IDungeonBuilder<Dungeon> AddRandomChambers(int numChambers)
    {
        const int minWidth = 3;
        const int minHeight = 3;
        for (var i = 0; i < numChambers; i++)
        {
            var position = new Position(Rng.Next(_dungeon.Width), Rng.Next(_dungeon.Height));
            var maxWidth = _dungeon.Width - position.X;
            var maxHeight = _dungeon.Height - position.Y;
            if (maxWidth < minWidth || maxHeight < minHeight)
            {
                i--;
                continue;
            }

            var width = Rng.Next(minWidth, maxWidth);
            var height = Rng.Next(minHeight, maxHeight);
            AddRoom(position, width, height);
        }

        return this;
    }

    public IDungeonBuilder<Dungeon> AddCentralRoom()
    {
        var width = _dungeon.Width / 3;
        var height = _dungeon.Height / 3;
        var position = new Position((_dungeon.Width - width) / 2, (_dungeon.Height - height) / 2);
        return AddRoom(position, width, height);
    }

    private readonly struct WallCandidate(Position wallPosition, Position cellPosition)
    {
        public Position WallPosition { get; } = wallPosition;
        public Position CellPosition { get; } = cellPosition;
    }

    public IDungeonBuilder<Dungeon> AddRandomPaths()
    {
        List<Position> directions =
        [
            (0, 2), (2, 0), (0, -2), (-2, 0)
        ];
        List<WallCandidate> candidates = [];

        var seed = _playerInitialPosition;

        foreach (var d in directions)
        {
            Position neighbor = (seed.X + d.X, seed.Y + d.Y);
            Position wall = (seed.X + d.X / 2, seed.Y + d.Y / 2);
            if (_dungeon.IsInBounds(neighbor) && !_dungeon[neighbor].IsPassable)
                candidates.Add(new WallCandidate(wall, neighbor));
        }

        while (candidates.Count > 0)
        {
            var index = Rng.Next(candidates.Count);
            var candidate = candidates[index];
            // move to last for faster removal
            candidates[index] = candidates[^1];
            candidates.RemoveAt(candidates.Count - 1);

            if (!_dungeon.IsInBounds(candidate.CellPosition) || _dungeon[candidate.CellPosition].IsPassable) continue;
            AddFloor(candidate.WallPosition);
            AddFloor(candidate.CellPosition);
            foreach (var d in directions)
            {
                Position neighbor = (candidate.CellPosition.X + d.X, candidate.CellPosition.Y + d.Y);
                Position wall = (candidate.CellPosition.X + d.X / 2, candidate.CellPosition.Y + d.Y / 2);
                if (_dungeon.IsInBounds(neighbor) && !_dungeon[neighbor].IsPassable)
                    candidates.Add(new WallCandidate(wall, neighbor));
            }
        }

        _dungeon[_playerInitialPosition] = new FloorTile();

        return this;
    }

    private DungeonBuilder AddItems<T>(Func<T>[] constructors, double probability, Func<T, T>[]? effects = null,
        int maxEffects = 0) where T : Item
    {
        foreach (var tile in _dungeon)
        {
            if (!tile.IsPassable || !(Rng.NextDouble() < probability)) continue;

            var item = constructors[Rng.Next(constructors.Length)]();
            if (effects != null)
                for (var i = 0; i < Rng.Next(maxEffects); i++)
                    item = effects[Rng.Next(effects.Length)](item);
            tile.Add(item);
        }

        return this;
    }

    public IDungeonBuilder<Dungeon> AddUnusableItems(double probability = 0.07) =>
        AddItems(_itemConstructors, probability, _itemEffectConstructors);

    public IDungeonBuilder<Dungeon> AddModifiedUnusableItems(double probability = 0.07, int maxEffects = 3) =>
        AddItems(_itemConstructors, probability, _itemEffectConstructors, maxEffects);

    public IDungeonBuilder<Dungeon> AddWeapons(double probability = 0.15) =>
        AddItems(_weaponConstructors, probability, _weaponEffectConstructors);
    
    public IDungeonBuilder<Dungeon> AddModifiedWeapons(double probability = 0.07, int maxEffects = 3) =>
        AddItems(_weaponConstructors, probability, _weaponEffectConstructors, maxEffects);

    public IDungeonBuilder<Dungeon> AddPotions(double probability = 0.15) =>
        AddItems(_potionConstructors, probability);

    public IDungeonBuilder<Dungeon> AddEnemies(double probability = 0.15)
    {
        foreach (var tile in _dungeon)
        {
            if (!tile.IsPassable || !(Rng.NextDouble() < probability)) continue;

            var enemy = _enemyConstructors[Rng.Next(_enemyConstructors.Length)]();
            tile.Add(enemy);
        }

        return this;
    }

    public IDungeonBuilder<Dungeon> AddMoney(double probability = 0.15) =>
        AddItems(_moneyConstructors, probability);

    public Dungeon Build()
    {
        _dungeon[_playerInitialPosition] = new FloorTile();
        return _dungeon;
    }
}