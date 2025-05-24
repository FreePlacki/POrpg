namespace POrpg.Commands;

public class RestCommand : ICommand
{
    public string Description => "You skipped a turn";
    public bool AdvancesTurn => true;
}