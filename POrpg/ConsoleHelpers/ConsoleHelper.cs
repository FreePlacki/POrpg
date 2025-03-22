using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using POrpg.Dungeon;

namespace POrpg.ConsoleHelpers;

public class ConsoleHelper
{
    private static ConsoleHelper? _instance;

    private (int margin, int width)[] _columns;

    private int ColumnStart(int columnIndex)
    {
        var result = _columns[0].margin;
        for (int i = 0; i < columnIndex; i++)
            result += _columns[i].width + _columns[i + 1].margin;
        return result;
    }

    private int[] _currentColumnHeights;
    private int[] _previousColumnHeights;
    private int _line;
    private int _column;
    private int _columnIndex;
    private readonly List<StringBuilder> _lines = new(60);
    public bool IsShowingInstructions { get; private set; }

    private ConsoleHelper((int margin, int width)[] columns)
    {
        Debug.Assert(_instance == null);

        _columns = columns;
        _currentColumnHeights = new int[_columns.Length];
        _previousColumnHeights = new int[_columns.Length];
    }

    public static ConsoleHelper Initialize((int margin, int width)[] columns)
    {
        _instance = new ConsoleHelper(columns);
        return _instance;
    }

    public static ConsoleHelper GetInstance()
    {
        Debug.Assert(_instance != null);
        return _instance;
    }

    public void ChangeColumn(int newColumnIndex)
    {
        _currentColumnHeights[_columnIndex] = _line;
        _columnIndex = newColumnIndex;
        SetCursorPosition(ColumnStart(_columnIndex), _currentColumnHeights[_columnIndex]);
    }

    public void ShowInstructions(Instructions instructions)
    {
        IsShowingInstructions = true;

        Reset();
        Console.Clear();
        var text = instructions.Build();

        var divider = new StyledText(new string('=', 10), Style.Faint).Text;
        Console.WriteLine($"{divider} {new StyledText("Instructions", Style.Underline).Text} {divider}\n");
        Console.WriteLine(text);
        Console.WriteLine(InputHint("?", "Hide instructions"));
    }

    public void HideInstructions()
    {
        IsShowingInstructions = false;
        Reset();
        Console.Clear();
    }

    public void Write(IConsoleText text)
    {
        var lines = text.Text.Split('\n').SelectMany(l => WrapAnsiString(l).Split('\n'));
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
        var padding = ColumnStart(_columnIndex) + _columns[_columnIndex].width - _column;
        if (padding > 0)
            WriteToBuffer(new string(' ', padding));
        SetCursorPosition(ColumnStart(_columnIndex), _line + 1);
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
        WriteLine(new StyledText(new string('\u2550', width), Style.Faint));
    }

    public void Reset()
    {
        for (var i = 0; i < _columns.Length; i++)
        {
            var s = new string(' ', _columns[i].width);
            for (var j = _currentColumnHeights[i]; j < _previousColumnHeights[i]; j++)
            {
                SetCursorPosition(ColumnStart(i), j);
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
        _column = ColumnStart(_columnIndex);
        _previousColumnHeights = _currentColumnHeights;
        _currentColumnHeights = new int[_columns.Length];
        SetCursorPosition(ColumnStart(_columnIndex), _line);
        Console.SetCursorPosition(ColumnStart(_columnIndex), _line);
    }

    public static string InputHint(string keys, string? description = null)
    {
        return description == null
            ? new StyledText(new StyledText(keys, Styles.Player), Style.Faint).Text
            : new StyledText($"{description} ({new StyledText(keys, Styles.Player).Text})", Style.Faint).Text;
    }

    private void SetCursorPosition(int column, int line)
    {
        _column = column;
        _line = line;
    }

    private void WriteToBuffer(string s)
    {
        if (string.IsNullOrEmpty(s)) return;

        while (_line >= _lines.Count)
            _lines.Add(new StringBuilder());

        var currentLine = _lines[_line];
        var visibleLength = GetVisibleLength(currentLine.ToString());

        if (_column < visibleLength - 1)
        {
            currentLine.Remove(_column, GetVisibleLength(s));
            currentLine.Insert(_column, s);
            _column += s.Length - GetVisibleLength(s);
        }
        else
        {
            if (visibleLength < _column)
                currentLine.Append(new string(' ', _column - visibleLength));
            currentLine.Append(s);
        }
    }

    private static int GetVisibleLength(string text)
    {
        var cleanedText = AnsiRegex.Replace(text, "");
        return cleanedText.Length;
    }

    private static readonly Regex AnsiRegex = new(@"\u001b\[[0-9;]*m", RegexOptions.Compiled);

    private string WrapAnsiString(string input)
    {
        var output = new StringBuilder();
        var activeSequences = new List<string>();
        int currentWidth = 0;
        int maxWidth = _columns[_columnIndex].width;

        var matches = AnsiRegex.Matches(input);
        int nextMatchIndex = 0;
        int inputIndex = 0;

        while (inputIndex < input.Length)
        {
            if (nextMatchIndex < matches.Count && matches[nextMatchIndex].Index == inputIndex)
            {
                string ansiCode = matches[nextMatchIndex].Value;
                output.Append(ansiCode);

                if (ansiCode == "\u001b[0m" || ansiCode == "\u001b[39m")
                    activeSequences.Clear();
                else
                    activeSequences.Add(ansiCode);

                inputIndex += ansiCode.Length;
                nextMatchIndex++;
                continue;
            }

            char c = input[inputIndex];
            if (c == '\n')
            {
                output.Append(c);
                currentWidth = 0;
                foreach (var seq in activeSequences)
                    output.Append(seq);
            }
            else
            {
                if (currentWidth >= maxWidth)
                {
                    output.Append('\n');
                    currentWidth = 0;
                    foreach (var seq in activeSequences)
                    {
                        output.Append(seq);
                    }
                }

                output.Append(c);
                currentWidth++;
            }

            inputIndex++;
        }

        return output.ToString();
    }
}