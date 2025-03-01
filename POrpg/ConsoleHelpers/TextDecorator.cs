namespace POrpg.ConsoleHelpers;

public abstract class TextDecorator : IConsoleText
{
    protected IConsoleText InnerText { get; }
    public string InitialText => InnerText.InitialText;

    protected TextDecorator(IConsoleText innerText) => InnerText = innerText;
    
    public abstract string Text { get; }
}