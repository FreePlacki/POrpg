namespace POrpg.Effects;

public abstract class Effect : ITurnObserver
{
    public abstract Attributes Attributes { get; }
    
    protected readonly int EndTurn;
    protected readonly Player Player;
    protected string _name;
    public readonly bool IsPermanent;

    public Effect(Player player, int duration, string name, bool isPermanent)
    {
        var tm = TurnManager.GetInstance();
        EndTurn = tm.Turn + duration;
        Player = player;
        _name = name;
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
        }
    }
}