using POrpg.ConsoleUtils;

namespace POrpg.Effects;

public abstract class Effect : ITurnObserver
{
    public abstract Attributes Attributes { get; }

    protected readonly int EndTurn;
    protected readonly Player Player;
    private readonly Dungeon.Dungeon _dungeon;
    public string Name { get; }
    public int TurnsLeft => EndTurn - _dungeon.TurnManager.Turn + 1;

    public string Description =>
        $"Turns left: {TurnsLeft}\n{Attributes.EffectDescription()}";

    public readonly bool IsPermanent;

    protected Effect(Dungeon.Dungeon dungeon, Player player, int? duration, string name, bool isPermanent)
    {
        _dungeon = dungeon;
        EndTurn = _dungeon.TurnManager.Turn + duration ?? -1;
        Player = player;
        Name = name;
        IsPermanent = isPermanent;

        player.AddEffect(this);
        _dungeon.TurnManager.RegisterObserver(this);
    }

    public void Update()
    {
        var tm = _dungeon.TurnManager;

        if (!IsPermanent && tm.Turn > EndTurn)
        {
            tm.UnregisterObserver(this);
            Player.RemoveEffect(this);
            ConsoleHelper.GetInstance().AddNotification($"{Name} effect expired");
        }
    }
}