namespace POrpg.ConsoleHelpers;

public enum Style : byte
{
    Reset = 0,
    Bold = 1,
    Faint = 2,
    Underline = 4,
    Black = 30,
    Red = 31,
    Green = 32,
    Yellow = 33,
    Blue = 34,
    Magenta = 35,
    Cyan = 36,
    White = 37,
    Normal = 39,

    Gradient,
    GoodBad,
}

public class StyledText : TextDecorator
{
    private readonly byte _colorCode;

    public StyledText(IConsoleText text, Style style) : base(text)
    {
        var s = style switch
        {
            Style.Gradient =>
                int.Parse(text.InitialText) switch
                {
                    <= 5 => Style.Red,
                    >= 10 => Style.Green,
                    _ => Style.Blue
                },
            Style.GoodBad => text.InitialText.StartsWith('-') ? Style.Red : Style.Green,
            _ => style
        };
        _colorCode = (byte)s;
    }

    public StyledText(string text, Style style) : this(new PlainText(text), style)
    {
    }

    public override string Text => $"\u001b[{_colorCode}m{InnerText.Text}\u001b[0m";
}