using POrpg.ConsoleHelpers;

namespace POrpg.Enemies;

public class Orc : Enemy
{
    public override string Symbol => new StyledText("O", Styles.Enemy).Text;
    public override string Name => "Orc";
}