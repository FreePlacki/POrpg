using System.Text.Json.Serialization;
using POrpg.Effects;

namespace POrpg;

public class TurnManager
{
    public int Turn { get; private set; }
    public List<Effect> Observers { get; }

    public void RegisterObserver(Effect observer) => Observers.Add(observer);
    public void UnregisterObserver(Effect observer) => Observers.Remove(observer);

    public TurnManager()
    {
        Observers = [];
    }

    [JsonConstructor]
    public TurnManager(int turn, List<Effect> observers)
    {
        Turn = turn;
        Observers = observers;
    }

    public void NextTurn()
    {
        Turn++;
        // a copy is needed because the observers can unregister during Update()
        var currentObservers = new List<Effect>(Observers);
        foreach (var observer in currentObservers)
            if (!observer.Update())
                UnregisterObserver(observer);
    }

    public void Reset()
    {
        Turn = 0;
        Observers.Clear();
    }
}