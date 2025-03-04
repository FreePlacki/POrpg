using System.Text;
using System.Text.RegularExpressions;

namespace POrpg.ConsoleHelpers;

public class ConsoleHelper
{
    private readonly (int start, int width)[] _columns;
    private int[] _currentColumnHeights;
    private int[] _previousColumnHeights;
    private int _line;
    private int _columnIndex;
    private readonly StringBuilder _buffer = new(50);

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
        _line = _currentColumnHeights[_columnIndex];
        Console.SetCursorPosition(_columns[_columnIndex].start, _line);
    }

    public void Write(IConsoleText text)
    {
        var lines = text.Text.Split('\n');
        var i = 0;
        foreach (var l in lines)
        {
            if (i != 0)
                WriteLine();
            _buffer.Append(l);
            i++;
        }
    }

    public void WriteLine(IConsoleText text)
    {
        Write(text);
        Console.Write(_buffer.ToString());
        // TODO: very slow
        var column = Console.CursorLeft;
        var padding = _columns[_columnIndex].start + _columns[_columnIndex].width - column;
        _buffer.Clear();
        if (padding > 0)
            Console.Write(new string(' ', padding));
        Console.SetCursorPosition(_columns[_columnIndex].start, ++_line);
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
                Console.SetCursorPosition(_columns[i].start, j);
                Console.Write(s);
            }
        }
        _line = 0;
        _columnIndex = 0;
        _previousColumnHeights = _currentColumnHeights;
        _currentColumnHeights = new int[_columns.Length];
        Console.SetCursorPosition(_columns[_columnIndex].start, _line);
    }
}