using POrpg.ConsoleHelpers;

namespace POrpg.Items;

public class TwoHandedSword : IWeapon
{
    public int Damage => 25;
    public string Symbol => new StyledText("S", Style.Cyan).Text;
    public string Name => "Two-Handed Sword";
    public bool IsTwoHanded => true;
}