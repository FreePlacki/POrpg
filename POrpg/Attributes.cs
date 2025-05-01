using System.Text;
using System.Text.Json.Serialization;
using POrpg.ConsoleHelpers;

namespace POrpg;

public enum Attribute
{
    Strength,
    Dexterity,
    Health,
    Luck,
    Aggression,
    Wisdom
}

public static class EnumExtensions
{
    public static string? EffectDescription(this Attributes? attributes)
    {
        if (attributes == null) return null;

        var result = new StringBuilder();
        foreach (var attribute in attributes.attributes)
        {
            if (attribute.Value == 0) continue;
            var value = new StyledText($"{attribute.Value:+#;-#}", Style.GoodBad);
            result.Append($"({value} {attribute.Key}) ");
        }

        return result.ToString().TrimEnd();
    }
}

public class Attributes
{
    [JsonInclude]
    public readonly Dictionary<Attribute, int> attributes;

    public Attributes(Dictionary<Attribute, int> attributes)
    {
        this.attributes = attributes;
    }

    [JsonIgnore]
    public int this[Attribute attribute]
    {
        get => attributes[attribute];
        set => attributes[attribute] = value;
    }
    
    [JsonIgnore]
    public bool IsEmpty => attributes.Count == 0;

    public static Attributes operator +(Attributes lhs, Attributes? rhs)
    {
        if (rhs == null) return lhs;

        var result = new Attributes(new());

        foreach (var (key, value) in lhs.attributes)
            result.Add(key, value);
        foreach (var (key, value) in rhs.attributes)
            result.Add(key, value);

        return result;
    }

    private void Add(Attribute attribute, int value)
    {
        if (!attributes.TryAdd(attribute, value))
            attributes[attribute] += value;
    }
}