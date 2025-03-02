using POrpg.ConsoleHelpers;

namespace POrpg.Items.Effects;

public class Legendary : Effect, IWeapon
{
    public Legendary(IItem item) : base(item)
    {
    }

    public override string Name => $"{Item.Name} ({new StyledText("Legendary", Style.Rainbow).Text})";

    Attributes? IItem.Attributes =>
        new Attributes(new Dictionary<Attribute, int>
        {
            { Attribute.Strength, 5 },
            { Attribute.Dexterity, 5 },
            { Attribute.Health, 5 },
            { Attribute.Luck, 5 },
            { Attribute.Aggression, 5 },
            { Attribute.Wisdom, 5 }
        }) + Item.Attributes;

    public int Damage => Item.Damage ?? throw new InvalidOperationException();
}