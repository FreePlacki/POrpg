namespace POrpg.ConsoleHelpers;

public interface IConsoleText
{
    string Text { get; }
    string InitialText { get; }
}

public class PlainText : IConsoleText
{
    public string Text { get; }
    public string InitialText { get; }

    public PlainText(string s)
    {
        Text = s;
        InitialText = s;
    }

    public static implicit operator PlainText(string s) => new(s);
}