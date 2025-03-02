namespace POrpg.Items;

public interface IWeapon : IItem
{
    new int Damage { get; }
    int? IItem.Damage => Damage;
}