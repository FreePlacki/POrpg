namespace POrpg.InputHandlers;

[Flags]
public enum UiLocation : byte
{
    None        = 0b00000000,
    Bottom      = 0b00000001,
    Inventory   = 0b00000010,
    Backpack    = 0b00000100,
    StandingOn  = 0b00001000,
    LookingAt   = 0b00010000
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