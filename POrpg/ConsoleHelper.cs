namespace POrpg;

public class ConsoleHelper
{
    public int Line { get; set; }
    public int Column { get; set; }

    public ConsoleHelper(int line = 0, int column = 0)
    {
        Line = line;
        Column = column;
    }

    public void Write(string text)
    {
        foreach (var l in text.Split('\n'))
        {
            Console.SetCursorPosition(Column, Line++);
            Console.Write(l);
        }
    }

    public void HorizontalDivider(int width = 30)
    {
        Write(new string('=', width));
    }
}