namespace POrpg.Commands;

public class MessageCommand : ICommand
{
    public string Description { get; private set; }
    private bool IncreasesCounter => false;

    public MessageCommand(string description) => Description = description;
    
    public void Execute()
    {
    }
}