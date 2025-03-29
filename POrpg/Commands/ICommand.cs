namespace POrpg.Commands;

public interface ICommand
{
    void Execute();
    string? Description => null;
    bool AdvancesTurn => Description != null;
}