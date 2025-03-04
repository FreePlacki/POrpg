using POrpg.ConsoleHelpers;

namespace POrpg.Items;

public class TwoHandedSword : Weapon
{
    public override int? Damage => 25;
    public override string Name => "Two-Handed Sword";
    public override bool IsTwoHanded => true;
}