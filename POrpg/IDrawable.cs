namespace POrpg;

public interface IDrawable
{
    // TODO: make Symbol just return the first character from Name
    string Symbol { get; }
    string Name { get; }
}