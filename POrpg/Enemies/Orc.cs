using System.Text.Json.Serialization;
using POrpg.ConsoleUtils;
using POrpg.Enemies.Behaviours;

namespace POrpg.Enemies;

public class Orc : Enemy
{
    public override string Symbol => new StyledText("O", Styles.Enemy).ToString();
    public override string Name => "Orc";
    public override int Damage => 10;
    public override int Health { get; set; }
    public override int Armor => 10;
    public override IBehaviour Behaviour { get; set; } = new AggressiveBehaviour();

    public Orc()
    {
        Health = 50;
    }

    [JsonConstructor]
    public Orc(int health)
    {
        Health = health;
    }
}