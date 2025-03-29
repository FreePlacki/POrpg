using POrpg.ConsoleHelpers;

namespace POrpg.Commands;

public class CommandQueue
{
    private readonly Queue<string> _commandLog = [];
    private readonly int _logSize;

    private ICommand? _lastCommand;

    public CommandQueue(int logSize)
    {
        _logSize = logSize;
    }

    public void Enqueue(ICommand command)
    {
        _lastCommand = command;
    }

    public void ExecuteLast()
    {
        if (_lastCommand == null) return;
        _lastCommand.Execute();
        if (_lastCommand.Description == null) return;
        _commandLog.Enqueue(_lastCommand.Description);
        if (_commandLog.Count > _logSize)
            _commandLog.Dequeue();
    }

    // TODO: move to consoleHelper as PrintNotifications and AddNotification
    public void Print()
    {
        var console = ConsoleHelper.GetInstance();
        for (var i = 0; i < _logSize - _commandLog.Count; i++)
            console.WriteLine();
        var j = 0;
        foreach (var msg in _commandLog)
        {
            if (j++ == _commandLog.Count - 1)
                console.WriteLine(msg);
            else
                console.WriteLine(new StyledText(msg, Style.Faint));
        }
    }
}