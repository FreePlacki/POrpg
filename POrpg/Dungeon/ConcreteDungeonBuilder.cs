using POrpg.Enemies;
using POrpg.Items;
using POrpg.Items.Effects;
using POrpg.Items.Effects.WeaponEffects;
using POrpg.Items.Potions;
using POrpg.Items.Weapons;

namespace POrpg.Dungeon;

public class ConcreteDungeonBuilder : DungeonBuilder
{
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
        () => new HealthPotion(), () => new StrengthPotion()
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

    public ConcreteDungeonBuilder(InitialDungeonState initialState, int width, int height,
        Position playerInitialPosition) : base(initialState, width, height, playerInitialPosition)
    {
    }

    private void AddFloor(Position position)
    {
        if (!Dungeon[position].IsPassable)
            Dungeon[position] = new FloorTile();
    }

    private ConcreteDungeonBuilder AddRoom(Position position, int width, int height)
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

    public override DungeonBuilder AddRandomChambers(int numChambers)
    {
        const int minWidth = 3;
        const int minHeight = 3;
        for (var i = 0; i < numChambers; i++)
        {
            var position = new Position(Rng.Next(Dungeon.Width), Rng.Next(Dungeon.Height));
            var maxWidth = Dungeon.Width - position.X;
            var maxHeight = Dungeon.Height - position.Y;
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

    public override DungeonBuilder AddCentralRoom()
    {
        var width = Dungeon.Width / 3;
        var height = Dungeon.Height / 3;
        var position = new Position((Dungeon.Width - width) / 2, (Dungeon.Height - height) / 2);
        return AddRoom(position, width, height);
    }

    private readonly struct WallCandidate(Position wallPosition, Position cellPosition)
    {
        public Position WallPosition { get; } = wallPosition;
        public Position CellPosition { get; } = cellPosition;
    }

    public override DungeonBuilder AddRandomPaths()
    {
        List<Position> directions =
        [
            (0, 2), (2, 0), (0, -2), (-2, 0)
        ];
        List<WallCandidate> candidates = [];

        var seed = PlayerInitialPosition;

        foreach (var d in directions)
        {
            Position neighbor = (seed.X + d.X, seed.Y + d.Y);
            Position wall = (seed.X + d.X / 2, seed.Y + d.Y / 2);
            if (Dungeon.IsInBounds(neighbor) && !Dungeon[neighbor].IsPassable)
                candidates.Add(new WallCandidate(wall, neighbor));
        }

        while (candidates.Count > 0)
        {
            var index = Rng.Next(candidates.Count);
            var candidate = candidates[index];
            // move to last for faster removal
            candidates[index] = candidates[^1];
            candidates.RemoveAt(candidates.Count - 1);

            if (!Dungeon.IsInBounds(candidate.CellPosition) || Dungeon[candidate.CellPosition].IsPassable) continue;
            AddFloor(candidate.WallPosition);
            AddFloor(candidate.CellPosition);
            foreach (var d in directions)
            {
                Position neighbor = (candidate.CellPosition.X + d.X, candidate.CellPosition.Y + d.Y);
                Position wall = (candidate.CellPosition.X + d.X / 2, candidate.CellPosition.Y + d.Y / 2);
                if (Dungeon.IsInBounds(neighbor) && !Dungeon[neighbor].IsPassable)
                    candidates.Add(new WallCandidate(wall, neighbor));
            }
        }

        Dungeon[PlayerInitialPosition] = new FloorTile();

        return this;
    }

    private ConcreteDungeonBuilder AddItems<T>(Func<T>[] constructors, double probability, Func<T, T>[]? effects = null,
        int maxEffects = 0) where T : Item
    {
        foreach (var tile in Dungeon)
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

    public override DungeonBuilder AddUnusableItems(double probability = 0.07, int maxEffects = 0)
    {
        Instructions.AddItems();
        return AddItems(_itemConstructors, probability, _itemEffectConstructors, maxEffects);
    }

    public override DungeonBuilder AddWeapons(double probability = 0.15, int maxEffects = 0)
    {
        Instructions.AddWeapons();
        return AddItems(_weaponConstructors, probability, _weaponEffectConstructors, maxEffects);
    }

    public override DungeonBuilder AddPotions(double probability = 0.15)
    {
        Instructions.AddPotions();
        return AddItems(_potionConstructors, probability);
    }

    public override DungeonBuilder AddEnemies(double probability = 0.15)
    {
        Instructions.AddEnemies();
        foreach (var tile in Dungeon)
        {
            if (!tile.IsPassable || !(Rng.NextDouble() < probability)) continue;

            var enemy = _enemyConstructors[Rng.Next(_enemyConstructors.Length)]();
            tile.Add(enemy);
        }

        return this;
    }

    public override DungeonBuilder AddMoney(double probability = 0.15)
    {
        Instructions.AddMoney();
        return AddItems(_moneyConstructors, probability);
    }

    public override Dungeon BuildDungeon()
    {
        Dungeon[PlayerInitialPosition] = new FloorTile();
        return Dungeon;
    }

    public override Instructions BuildInstructions() => Instructions;
}