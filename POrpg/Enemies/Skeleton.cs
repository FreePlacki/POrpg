using POrpg.ConsoleHelpers;

namespace POrpg.Enemies;

public class Skeleton : Enemy
{
    public override string Symbol => new StyledText("S", Styles.Enemy).Text;
    public override string Name => "Skeleton";
    public override int Damage => 5;
}