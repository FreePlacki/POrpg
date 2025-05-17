using POrpg.ConsoleUtils;

namespace POrpg.Enemies;

public class Orc : Enemy
{
    public override string Symbol => new StyledText("O", Styles.Enemy).ToString();
    public override string Name => "Orc";
    public override int Damage => 10;
    public override int Health { get; protected set; } = 50;
    public override int Armor => 10;
}