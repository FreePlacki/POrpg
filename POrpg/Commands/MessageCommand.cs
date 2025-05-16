namespace POrpg.Commands;

public class MessageCommand : ICommand
{
    public string Description { get; }
    public bool AdvancesTurn => false;

    public MessageCommand(string description) => Description = description;
}