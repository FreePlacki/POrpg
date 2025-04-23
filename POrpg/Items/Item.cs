using System.Text;
using POrpg.ConsoleHelpers;
using POrpg.Inventory;
using POrpg.Items.Weapons;

namespace POrpg.Items;

public abstract class Item : IDrawable
{
    public virtual Attributes? Attributes => null;
    public virtual EquipmentSlotType EquipmentSlotType => EquipmentSlotType.LeftHand | EquipmentSlotType.RightHand;

    public virtual bool OnPickUp(Player player) => false;

    public virtual (int damage, int defense) Accept(IAttackVisitor visitor) => visitor.Visit(this);

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

    public virtual string Symbol => new StyledText(Name.First().ToString(), Styles.Item).ToString();
    public abstract string Name { get; }
}