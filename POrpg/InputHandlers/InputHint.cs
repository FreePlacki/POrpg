namespace POrpg.InputHandlers;

[Flags]
public enum UiLocation : byte
{
    None = 0b0000,
    Bottom = 0b0001,
    Inventory = 0b0010,
    Backpack = 0b0100,
    StandingOn = 0b1000,
}

public class InputHint
{
    public string Key { get; }
    public string Description { get; }
    public UiLocation Location { get; }

    public InputHint(string key, string description, UiLocation location)
    {
        Key = key;
        Description = description;
        Location = location;
    }
}