using System.Text;
using POrpg.Combat;
using POrpg.ConsoleHelpers;
using POrpg.Inventory;

namespace POrpg.Items;

public abstract class Item : IDrawable
{
    public virtual Attributes? Attributes => null;
    public virtual EquipmentSlotType EquipmentSlotType => EquipmentSlotType.LeftHand | EquipmentSlotType.RightHand;

    public virtual bool OnPickUp(Player player) => false;

    // TODO: why the following doesn't work?
    // public virtual int Accept(IAttackVisitor visitor) => visitor.Visit(this);
    public virtual (int damage, int defense) Accept(IAttackVisitor visitor) => (0, 0);

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

    public virtual string Symbol => new StyledText("I", Styles.Item).ToString();
    public abstract string Name { get; }
}