namespace POrpg.Commands;

public interface ICommand
{
    bool IncreasesCounter { get; }
    void Execute();
}