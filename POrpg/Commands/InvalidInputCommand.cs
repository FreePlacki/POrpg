namespace POrpg.Commands;

public class InvalidInputCommand : ICommand
{
    public string Description => "Invalid input";
    private bool IncreasesCounter => false;

    public void Execute()
    {
    }
}