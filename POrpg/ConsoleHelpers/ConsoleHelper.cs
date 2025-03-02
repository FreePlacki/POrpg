using System.Text;

namespace POrpg.ConsoleHelpers;

public class ConsoleHelper
{
    public int Line { get; set; }

    private int _column;
    private readonly StringBuilder _buffer = new();

    public int Column
    {
        get => _column;
        set
        {
            _column = value;
            _buffer.Clear();
        }
    }

    public ConsoleHelper(int line = 0, int column = 0)
    {
        Line = line;
        Column = column;
    }

    public void Write(IConsoleText text)
    {
        var lines = text.Text.Split('\n');
        var i = 0;
        foreach (var l in lines)
        {
            if (i != 0)
                WriteLine();
            i++;
            _buffer.Append(l);
        }
    }

    public void WriteLine(IConsoleText text)
    {
        Write(text);
        Console.SetCursorPosition(Column, Line);
        Console.Write(_buffer.ToString());
        _buffer.Clear();
        Line++;
    }

    public void Write(string? text)
    {
        if (text == null) return;
        Write(new PlainText(text));
    }

    public void WriteLine(string? text = "")
    {
        if (text == null) return;
        WriteLine(new PlainText(text));
    }

    public void HorizontalDivider(int width = 30)
    {
        WriteLine(new StyledText(new string('=', width), Style.Faint));
    }

    // [GeneratedRegex(@"\u001b\[[0-9;]+m", RegexOptions.Compiled)]
    // private static partial Regex AnsiRegex();
    //
    // private static int GetVisibleLength(string text)
    // {
    //     var cleanedText = AnsiRegex().Replace(text, "");
    //     return cleanedText.Length;
    // }
}