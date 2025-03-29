namespace POrpg;

public interface ITurnObserver
{
    void Update();
}

public class TurnManager
{
    public int Turn { get; private set; }
    private readonly List<ITurnObserver> _observers = [];
    private static TurnManager? _instance;
    
    public static TurnManager GetInstance() => _instance ??= new TurnManager();
    
    public void RegisterObserver(ITurnObserver observer) => _observers.Add(observer);
    public void UnregisterObserver(ITurnObserver observer) => _observers.Remove(observer);

    public void NextTurn()
    {
        Turn++;
        // a copy is needed because the observers can unregister during Update()
        var currentObservers = new List<ITurnObserver>(_observers);
        foreach (var observer in currentObservers)
            observer.Update();
    }
}