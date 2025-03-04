using System.Text;

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
    Rainbow,
}

public class StyledText : TextDecorator
{
    private readonly Style _style;
    private readonly byte _reset;

    public StyledText(IConsoleText text, Style style) : base(text)
    {
        _style = style switch
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
        if (_style != Style.Faint && _style != Style.Underline) _reset = (byte)Style.Normal;
    }

    public StyledText(string text, Style style) : this(new PlainText(text), style)
    {
    }

    public override string Text
    {
        get
        {
            if (_style == Style.Rainbow)
            {
                var sb = new StringBuilder();
                int i = (byte)Style.Red;
                foreach (var c in InnerText.Text)
                {
                    sb.Append($"\u001b[{i}m{c}\u001b[0m");
                    i++;
                    if (i > (byte)Style.Cyan) i = (byte)Style.Red;
                }

                return sb.ToString();
            }

            return $"\u001b[{(byte)_style}m{InnerText.Text}\u001b[{_reset}m";
        }
    }
}