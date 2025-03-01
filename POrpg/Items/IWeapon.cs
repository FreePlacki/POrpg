namespace POrpg.Items;

public interface IWeapon : IItem
{
    bool IsTwoHanded => false;
    new int Damage { get; }
    int? IItem.Damage => Damage;
}