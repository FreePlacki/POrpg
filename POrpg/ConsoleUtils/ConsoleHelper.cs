using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using POrpg.InputHandlers;

namespace POrpg.ConsoleUtils;

public class ConsoleHelper
{
    private static ConsoleHelper? _instance;

    private readonly (int margin, int width)[] _columns;
    private readonly int _logSize;

    private int[] _currentColumnHeights;
    private int[] _previousColumnHeights;
    private int _line;
    private int _column;
    private int _columnIndex;
    private readonly List<StringBuilder> _lines = new(60);
    public bool IsShowingInstructions { get; private set; }
    private readonly string _instructions;
    private readonly Queue<string> _notifications = new();

    private ConsoleHelper(string instructions, (int margin, int width)[] columns, int logSize)
    {
        Debug.Assert(_instance == null);

        _instructions = instructions;
        _columns = columns;
        _currentColumnHeights = new int[_columns.Length];
        _previousColumnHeights = new int[_columns.Length];
        _logSize = logSize;
    }

    public static ConsoleHelper Initialize(string instructions, (int margin, int width)[] columns, int logSize)
    {
        _instance = new ConsoleHelper(instructions, columns, logSize);
        return _instance;
    }

    public static ConsoleHelper GetInstance()
    {
        Debug.Assert(_instance != null);
        return _instance;
    }

    private int ColumnStart(int columnIndex)
    {
        var result = _columns[0].margin;
        for (int i = 0; i < columnIndex; i++)
            result += _columns[i].width + _columns[i + 1].margin;
        return result;
    }

    public void ChangeColumn(int newColumnIndex)
    {
        _currentColumnHeights[_columnIndex] = _line;
        _columnIndex = newColumnIndex;
        SetCursorPosition(ColumnStart(_columnIndex), _currentColumnHeights[_columnIndex]);
    }

    public void AddNotification(string notification)
    {
        _notifications.Enqueue(notification);
        if (_notifications.Count > _logSize)
            _notifications.Dequeue();
    }

    public void PrintNotifications()
    {
        for (var i = 0; i < _logSize - _notifications.Count; i++)
            WriteLine();
        var j = 0;
        foreach (var msg in _notifications)
        {
            if (j++ == _notifications.Count - 1)
                WriteLine(msg);
            else
                WriteLine(new StyledText(msg, Style.Faint));
        }
    }

    public void ShowInstructions()
    {
        IsShowingInstructions = true;

        Reset();
        Console.Clear();

        var divider = new StyledText(new string('=', 10), Style.Faint);
        Console.WriteLine($"{divider} {new StyledText("Instructions", Style.Underline)} {divider}\n");
        Console.WriteLine(_instructions);
        WriteHintLine(new InputHint("?", "Hide instructions", UiLocation.None));
    }

    public void HideInstructions()
    {
        IsShowingInstructions = false;
        Reset();
        Console.Clear();
    }

    public void ShowDeathScreen()
    {
        Reset();
        Console.Clear();
        WriteLine("You died!");
        WriteLine();
        WriteLine(new StyledText("Press any key to continue", Style.Faint));
        Reset();
    }

    public void Write(StyledText text)
    {
        var lines = text.ToString().Split('\n').SelectMany(l => WrapAnsiString(l).Split('\n'));
        var i = 0;
        foreach (var l in lines)
        {
            if (i != 0)
                WriteLine();
            WriteToBuffer(l);
            i++;
        }
    }

    public void WriteLine(StyledText text)
    {
        Write(text);
        var padding = ColumnStart(_columnIndex) + _columns[_columnIndex].width - _column;
        var space = padding > 0 ? new string(' ', padding) : "";
        var dividerLine = _line < _lines.Count && _lines[_line][^5] == '━';
        var sign = dividerLine ? "╋" : "┃";
        var extraSpace = dividerLine ? new StyledText("━", Style.Faint).ToString() : " ";
        WriteToBuffer($"{space}{extraSpace}{new StyledText(sign, Style.Faint)}");
        SetCursorPosition(ColumnStart(_columnIndex), _line + 1);
    }

    public void Write(string? text)
    {
        if (text == null) return;
        Write(new StyledText(text));
    }

    public void WriteLine(string? text = "")
    {
        if (text == null) return;
        WriteLine(new StyledText(text));
    }

    public void HorizontalDivider()
    {
        var width = _columns[_columnIndex].width;
        if (width <= 0) return;

        // -5 for the previous style (Faint)!
        _lines[_line][^5] = '╋';
        _lines[_line].Append(new StyledText("━", Style.Faint));
        WriteLine(new StyledText($"{new string('━', width)}", Style.Faint));
    }

    public void Reset()
    {
        ChangeColumn(0); // to save the _currentColumnHeights
        for (var i = 0; i < _columns.Length; i++)
        {
            var s = new string(' ', _columns[i].width + 20);
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
        _previousColumnHeights = _currentColumnHeights.ToArray();
        _currentColumnHeights = new int[_columns.Length];
        SetCursorPosition(ColumnStart(_columnIndex), _line);
        Console.SetCursorPosition(ColumnStart(_columnIndex), _line);
    }

    public void WriteHint(InputHint hint)
    {
        Write(new StyledText($"{hint.Description} ({new StyledText(hint.Key, Styles.Player)})", Style.Faint));
    }

    public void WriteHintLine(InputHint hint)
    {
        WriteHint(hint);
        WriteLine();
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
            currentLine.Remove(_column, s.Length);
            currentLine.Insert(_column, s);
            // _column += s.Length - GetVisibleLength(s);
        }
        else
        {
            if (visibleLength < _column)
                currentLine.Append(new string(' ', _column - visibleLength));
            currentLine.Append(s);
        }

        _column += GetVisibleLength(s);
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