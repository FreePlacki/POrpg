using System.Text;

namespace POrpg.Items;

public interface IItem : IDrawable
{
    public Attributes? Attributes => null;
    public int? Damage => null;
    
    string? IDrawable.Description
    {
        get
        {
            var sb = new StringBuilder();
            if (Damage != null)
            {
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