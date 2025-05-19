using POrpg.ConsoleUtils;

namespace POrpg.Effects;

public abstract class Effect
{
    public abstract Attributes Attributes { get; }
    public abstract string Name { get; }
    public abstract int Duration { get; set; }
    public abstract bool IsPermanent { get; }

    public int PlayerId { get; }

    public string Description =>
        $"Turns left: {Duration}\n{Attributes.EffectDescription()}";

    public Effect(int playerId)
    {
        PlayerId = playerId;
    }

    public bool Update()
    {
        Duration--;
        if (!IsPermanent && Duration <= 0)
        {
            ConsoleHelper.GetInstance().AddNotification($"{Name} effect expired");
            return false;
        }

        return true;
    }
}