using System.Text;
using POrpg.Inventory;

namespace POrpg.Items;

public abstract class Item : IDrawable
{
    public virtual Attributes? Attributes => null;
    public virtual int? Damage => null;
    public virtual EquipmentSlotType EquipmentSlotType => EquipmentSlotType.None;

    public virtual bool OnPickUp(Player player) => false;

    public string? Description
    {
        get
        {
            var sb = new StringBuilder();
            if (Damage != null)
                sb.AppendLine($"Damage: {Damage}");
            if (Attributes?.IsEmpty == false)
                sb.Append($"Effects: {Attributes.EffectDescription()}");

            var desc = sb.ToString().Trim();
            return string.IsNullOrWhiteSpace(desc) ? null : desc;
        }
    }
    
    public abstract string Symbol { get; }
    public abstract string Name { get; }
}