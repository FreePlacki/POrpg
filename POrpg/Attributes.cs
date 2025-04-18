using System.Collections;
using System.Text;
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
        foreach (var attribute in attributes)
        {
            if (attribute.Value == 0) continue;
            var value = new StyledText($"{attribute.Value:+#;-#}", Style.GoodBad).Text;
            result.Append($"({value} {attribute.Key}) ");
        }

        return result.ToString().TrimEnd();
    }
}

public class Attributes : IEnumerable<KeyValuePair<Attribute, int>>
{
    private readonly Dictionary<Attribute, int> _attributes;

    public Attributes(Dictionary<Attribute, int> attributes)
    {
        _attributes = attributes;
    }

    public int this[Attribute attribute]
    {
        get => _attributes[attribute];
        set => _attributes[attribute] = value;
    }
    
    public bool IsEmpty => _attributes.Count == 0;

    public static Attributes operator +(Attributes lhs, Attributes? rhs)
    {
        if (rhs == null) return lhs;

        var result = new Attributes(new());

        foreach (var (key, value) in lhs)
            result.Add(key, value);
        foreach (var (key, value) in rhs)
            result.Add(key, value);

        return result;
    }

    private void Add(Attribute attribute, int value)
    {
        if (!_attributes.TryAdd(attribute, value))
            _attributes[attribute] += value;
    }

    public IEnumerator<KeyValuePair<Attribute, int>> GetEnumerator() => _attributes.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}