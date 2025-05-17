using POrpg.ConsoleUtils;

namespace POrpg.Items.Modifiers;

public class Legendary : Modifier
{
    public Legendary(Item item) : base(item)
    {
    }

    public override string Name => $"{Item.Name} ({new StyledText("Legendary", Style.Rainbow)})";

    public override Attributes Attributes =>
        new Attributes(new Dictionary<Attribute, int>
        {
            { Attribute.Strength, 5 },
            { Attribute.Dexterity, 5 },
            { Attribute.Health, 5 },
            { Attribute.Luck, 5 },
            { Attribute.Aggression, 5 },
            { Attribute.Wisdom, 5 }
        }) + Item.Attributes;
}