namespace POrpg.Items;

public class Sword : IWeapon
{
    public int Damage => 10;
    public char Symbol => 'S';
    public string Name => "Sword";
}