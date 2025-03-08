using System.Text;
using System.Text.RegularExpressions;

namespace POrpg.ConsoleHelpers;

public partial class ConsoleHelper
{
    private readonly (int start, int width)[] _columns;
    private int[] _currentColumnHeights;
    private int[] _previousColumnHeights;
    private int _line;
    private int _column;
    private int _columnIndex;
    private readonly List<StringBuilder> _lines = new(60);

    public ConsoleHelper((int start, int width)[] columns)
    {
        _columns = columns;
        _currentColumnHeights = new int[_columns.Length];
        _previousColumnHeights = new int[_columns.Length];
    }

    public void ChangeColumn(int newColumnIndex)
    {
        _currentColumnHeights[_columnIndex] = _line;
        _columnIndex = newColumnIndex;
        SetCursorPosition(_columns[_columnIndex].start, _currentColumnHeights[_columnIndex]);
    }

    public void Write(IConsoleText text)
    {
        var lines = text.Text.Split('\n');
        var i = 0;
        foreach (var l in lines)
        {
            if (i != 0)
                WriteLine();
            WriteToBuffer(l);
            _column += GetVisibleLength(l);
            i++;
        }
    }

    public void WriteLine(IConsoleText text)
    {
        Write(text);
        var padding = _columns[_columnIndex].start + _columns[_columnIndex].width - _column;
        WriteToBuffer(new string(' ', padding));
        SetCursorPosition(_columns[_columnIndex].start, _line + 1);
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

    public void Reset()
    {
        for (var i = 0; i < _columns.Length; i++)
        {
            var s = new string(' ', _columns[i].width);
            for (var j = _currentColumnHeights[i]; j < _previousColumnHeights[i]; j++)
            {
                SetCursorPosition(_columns[i].start, j);
                WriteToBuffer(s);
            }
        }

        foreach (var sb in _lines)
        {
            Console.WriteLine(sb.ToString());
        }

        _lines.Clear();

        _line = 0;
        _columnIndex = 0;
        _column = _columns[_columnIndex].start;
        _previousColumnHeights = _currentColumnHeights;
        _currentColumnHeights = new int[_columns.Length];
        SetCursorPosition(_columns[_columnIndex].start, _line);
        Console.SetCursorPosition(_columns[_columnIndex].start, _line);
    }
    
    private void SetCursorPosition(int column, int line)
    {
        _column = column;
        _line = line;
    }
    
    private void WriteToBuffer(string s)
    {
        while (_line >= _lines.Count)
            _lines.Add(new StringBuilder());
        var len = GetVisibleLength(_lines[_line].ToString());
        if (len < _column) _lines[_line].Append(new string(' ', _column - len));
        _lines[_line].Append(s);
    }

    [GeneratedRegex(@"\u001b\[[0-9;]+m", RegexOptions.Compiled)]
    private static partial Regex AnsiRegex();

    private static int GetVisibleLength(string text)
    {
        var cleanedText = AnsiRegex().Replace(text, "");
        return cleanedText.Length;
    }
}