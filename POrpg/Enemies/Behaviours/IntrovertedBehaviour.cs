using POrpg.Dungeon;
using POrpg.Enemies.Decisions;

namespace POrpg.Enemies.Behaviours;

public class IntrovertedBehaviour : IBehaviour
{
    private readonly AggressiveBehaviour _aggressiveBehaviour = new();

    public Decision ComputeAction(Position position, Dungeon.Dungeon dungeon)
    {
        var target = dungeon.Players.Values
            .FirstOrDefault(p => p.Position.Distance(position) == 1)
            ?.Position;

        if (target != null && target.Value.Distance(position) == 1)
            return new MoveDecision(position, position - target.Value);
        
        var decision = _aggressiveBehaviour.ComputeAction(position, dungeon);
        if (decision is MoveDecision moveDecision)
            return new MoveDecision(moveDecision.Position, -1 * moveDecision.Direction);

        return decision;
    }
}