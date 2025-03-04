using POrpg.ConsoleHelpers;

namespace POrpg.Items;

public class Sword : Weapon
{
    public override int? Damage => 10;
    public override string Name => "Sword";
}