using POrpg.ConsoleHelpers;

namespace POrpg.Items;

public class TwoHandedSword : IWeapon
{
    public int Damage => 25;
    public string Symbol => new StyledText(new StyledText("S", Style.Cyan), Style.Underline).Text;
    public string Name => "Two-Handed Sword";
    public bool IsTwoHanded => true;
}