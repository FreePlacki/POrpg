using POrpg.Items;
using POrpg.Items.Effects;
using POrpg.Items.Effects.WeaponEffects;

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

    private readonly Func<Item, Item>[] _itemEffectConstructors =
    [
        i => new Legendary(i), i => new Unlucky(i), i => new TwoHanded(i)
    ];

    private readonly Func<Weapon, Weapon>[] _weaponEffectConstructors =
    [
        w => new LegendaryWeapon(w), w => new UnluckyWeapon(w),
        w => new Powerful(w), w => new TwoHandedWeapon(w)
    ];

    public ConcreteDungeonBuilder(InitialDungeonState initialState, int width, int height) : base(initialState, width,
        height)
    {
    }

    public override DungeonBuilder AddRandomPaths(int numPaths)
    {
        throw new NotImplementedException();
    }

    private ConcreteDungeonBuilder AddRoom(Position position, int width, int height)
    {
        for (var y = position.Y; y < position.Y + height; y++)
        {
            for (var x = position.X; x < position.X + width; x++)
            {
                Dungeon[(x, y)] = new FloorTile();
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
            var position = new Position(_rng.Next(Dungeon.Width), _rng.Next(Dungeon.Height));
            var maxWidth = Dungeon.Width - position.X;
            var maxHeight = Dungeon.Height - position.Y;
            if (maxWidth < minWidth || maxHeight < minHeight)
            {
                i--;
                continue;
            }

            var width = _rng.Next(minWidth, maxWidth);
            var height = _rng.Next(minHeight, maxHeight);
            AddRoom(position, width, height);
        }

        return this;
    }

    public override DungeonBuilder AddCentralRoom()
    {
        var width = Dungeon.Width / 2;
        var height = Dungeon.Height / 2;
        var position = new Position((Dungeon.Width - width) / 2, (Dungeon.Height - height) / 2);
        return AddRoom(position, width, height);
    }

    private ConcreteDungeonBuilder AddItems<T>(Func<T>[] constructors, Func<T, T>[] effects, double probability,
        int maxEffects) where T : Item
    {
        foreach (var tile in Dungeon)
        {
            if (!tile.IsPassable || !(_rng.NextDouble() < probability)) continue;

            var item = constructors[_rng.Next(constructors.Length)]();
            for (var i = 0; i < _rng.Next(maxEffects); i++)
                item = effects[_rng.Next(effects.Length)](item);
            tile.Add(item);
        }

        return this;
    }

    public override DungeonBuilder AddUnusableItems(double probability = 0.07, int maxEffects = 0) =>
        AddItems(_itemConstructors, _itemEffectConstructors, probability, maxEffects);

    public override DungeonBuilder AddWeapons(double probability = 0.15, int maxEffects = 0) =>
        AddItems(_weaponConstructors, _weaponEffectConstructors, probability, maxEffects);

    public override DungeonBuilder AddMoney(double probability = 0.15) =>
        AddItems(_moneyConstructors, _itemEffectConstructors, probability, 0);

    public override Dungeon Build() => Dungeon;
}