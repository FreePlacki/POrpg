using System.Text.Json.Serialization;
using POrpg.ConsoleUtils;
using POrpg.Enemies.Behaviours;

namespace POrpg.Enemies;

public class Skeleton : Enemy
{
    public override string Symbol => new StyledText("S", Styles.Enemy).ToString();
    public override string Name => "Skeleton";
    public override int Damage => 5;
    public override int Health { get; protected set; }
    public override int Armor => 0;
    protected override IBehaviour Behaviour { get; } = new CalmBehaviour();

    public Skeleton()
    {
        Health = 100;
    }

    [JsonConstructor]
    public Skeleton(int health)
    {
        Health = health;
    }
}