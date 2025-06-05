using System.Text.Json.Serialization;
using POrpg.ConsoleUtils;
using POrpg.Dungeon;
using POrpg.Enemies.Behaviours;
using POrpg.Enemies.Decisions;

namespace POrpg.Enemies;

public class Skeleton : Enemy
{
    public override string Symbol => new StyledText("S", Styles.Enemy).ToString();
    public override string Name => "Skeleton";
    public override int Damage => 5;
    private int _health;

    public override int Health
    {
        get => _health;
        set
        {
            var threashold = 50;
            if (_health > threashold && value <= threashold)
                Behaviour = new ShyBehaviour();
            _health = value;
        }
    }
    public override int Armor => 0;
    public override IBehaviour Behaviour { get; set; } = new AggressiveBehaviour();

    public Skeleton()
    {
        Health = 100;
    }

    [JsonConstructor]
    public Skeleton(int health, IBehaviour behaviour)
    {
        Health = health;
        Behaviour = behaviour;
    }
}