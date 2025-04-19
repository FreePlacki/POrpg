namespace POrpg.ConsoleHelpers;

public abstract class TextDecorator
{
    protected TextDecorator InnerText { get; }
    public string InitialText => InnerText.InitialText;

    protected TextDecorator(TextDecorator innerText) => InnerText = innerText;
    
    public abstract string Text { get; }
}