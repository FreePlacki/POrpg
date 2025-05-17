using POrpg.ConsoleUtils;

namespace POrpg.Effects;

public abstract class Effect : ITurnObserver
{
    public abstract Attributes Attributes { get; }

    protected readonly int EndTurn;
    protected readonly Player Player;
    public string Name { get; }
    public int TurnsLeft => EndTurn - TurnManager.GetInstance().Turn + 1;

    public string Description =>
        $"Turns left: {TurnsLeft}\n{Attributes.EffectDescription()}";

    public readonly bool IsPermanent;

    protected Effect(Player player, int? duration, string name, bool isPermanent)
    {
        var tm = TurnManager.GetInstance();
        EndTurn = tm.Turn + duration ?? -1;
        Player = player;
        Name = name;
        IsPermanent = isPermanent;

        player.AddEffect(this);
        tm.RegisterObserver(this);
    }

    public void Update()
    {
        var tm = TurnManager.GetInstance();

        if (!IsPermanent && tm.Turn > EndTurn)
        {
            tm.UnregisterObserver(this);
            Player.RemoveEffect(this);
            ConsoleHelper.GetInstance().AddNotification($"{Name} effect expired");
        }
    }
}