using System.Text;

namespace POrpg.Items;

public interface IItem : IDrawable
{
    public Attributes? Attributes => null;
    public int? Damage => null;
    public bool IsTwoHanded => false;
    public bool OnPickUp(Player player) { return false; }
    
    string? Description
    {
        get
        {
            var sb = new StringBuilder();
            if (Damage != null)
            {
                if (IsTwoHanded)
                    sb.AppendLine("(Two-Handed)");
                sb.AppendLine($"Damage: {Damage}");
            }
            if (Attributes != null && Attributes.Any())
            {
                sb.Append($"Effects: {Attributes.EffectDescription()}");
            }
            var desc = sb.ToString().Trim();
            return string.IsNullOrWhiteSpace(desc) ? null : desc;
        }
    }
}