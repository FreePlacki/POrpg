using System.Text;

namespace POrpg.ConsoleUtils;

public enum Style : byte
{
    Reset = 0,
    Bold = 1,
    Faint = 2,
    Italic = 3,
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

static class Styles
{
    public const Style Player = Style.Magenta;
    public const Style Item = Style.White;
    public const Style Weapon = Style.Cyan;
    public const Style Potion = Style.Blue;
    public const Style Enemy = Style.Red;
    public const Style Money = Style.Yellow;
    public const Style Stacked = Style.Underline;
    public const Style Effect = Style.Italic;
}

public class StyledText
{
    private readonly Style? _style;
    private readonly byte _reset;
    private StyledText? _innerText;
    private string? _text;
    public string InitialText => _innerText?.InitialText ?? _text!;

    public StyledText(StyledText text, params Style[] styles)
    {
        var t = text;
        foreach (var style in styles)
            t = new StyledText(t, style);
        _style = t._style;
        _reset = t._reset;
        _innerText = t._innerText;
        _text = t._text;
    }

    public StyledText(string text, params Style[] styles) : this(new StyledText(text), styles)
    {
    }

    // TODO: refactor to accept varargs styles
    public StyledText(StyledText text, Style style)
    {
        _innerText = text;

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
        if (_style >= Style.Black) _reset = (byte)Style.Normal;
    }

    public StyledText(string text)
    {
        _text = text;
    }

    public StyledText(string text, Style style) : this(new StyledText(text), style)
    {
    }

    public override string ToString()
    {
        var inner = _innerText?.ToString() ?? _text!;
        if (_style == null)
            return inner;

        if (_style == Style.Rainbow)
        {
            var sb = new StringBuilder();
            int i = (byte)Style.Red;
            foreach (var c in inner)
            {
                sb.Append($"\u001b[{i}m{c}\u001b[{_reset}m");
                i++;
                if (i > (byte)Style.Cyan) i = (byte)Style.Red;
            }

            return sb.ToString();
        }

        return $"\u001b[{(byte)_style}m{inner}\u001b[{_reset}m";
    }
}