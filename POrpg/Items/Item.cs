using System.Text;
using POrpg.ConsoleHelpers;
using POrpg.Inventory;

namespace POrpg.Items;

public abstract class Item : IDrawable
{
    public virtual Attributes? Attributes => null;
    public virtual EquipmentSlotType EquipmentSlotType => EquipmentSlotType.None;

    public virtual bool OnPickUp(Player player) => false;

    public virtual string? Description
    {
        get
        {
            var sb = new StringBuilder();
            if (Attributes?.IsEmpty == false)
                sb.Append($"Effects: {Attributes.EffectDescription()}");

            var desc = sb.ToString().Trim();
            return string.IsNullOrWhiteSpace(desc) ? null : desc;
        }
    }

    public virtual string Symbol => new StyledText("I", Style.Blue).Text;
    public abstract string Name { get; }
}