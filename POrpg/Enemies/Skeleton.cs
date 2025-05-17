using POrpg.ConsoleUtils;

namespace POrpg.Enemies;

public class Skeleton : Enemy
{
    public override string Symbol => new StyledText("S", Styles.Enemy).ToString();
    public override string Name => "Skeleton";
    public override int Damage => 5;
    public override int Health { get; protected set; } = 100;
    public override int Armor => 0;
}