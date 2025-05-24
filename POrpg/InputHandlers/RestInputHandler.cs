using POrpg.Commands;

namespace POrpg.InputHandlers;

public class RestInputHandler : InputHandler
{
    public override ICommand HandleInput(ConsoleKeyInfo keyInfo)
    {
        return keyInfo.Key switch
        {
            ConsoleKey.T => new RestCommand(),
            _ => NextHandler!.HandleInput(keyInfo)
        };
    }

    public override IEnumerable<InputHint> GetHints()
    {
        yield return new InputHint("T", "Rest", UiLocation.Bottom);

        foreach (var hint in NextHandler!.GetHints())
            yield return hint;
    }
}