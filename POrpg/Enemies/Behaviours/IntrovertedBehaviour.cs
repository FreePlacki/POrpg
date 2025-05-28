using POrpg.Dungeon;
using POrpg.Enemies.Decisions;

namespace POrpg.Enemies.Behaviours;

public class IntrovertedBehaviour : IBehaviour
{
    private AggressiveBehaviour _aggressiveBehaviour = new();

    public Decision ComputeAction(Position position, Dungeon.Dungeon dungeon)
    {
        var decision = _aggressiveBehaviour.ComputeAction(position, dungeon);
        if (decision is MoveDecision moveDecision)
        {
            return new MoveDecision(moveDecision.Position, moveDecision.Direction switch
            {
                { X: 1, Y: 0 } => new Position(-1, 0),
                { X: -1, Y: 0 } => new Position(1, 0),
                { X: 0, Y: 1 } => new Position(0, -1),
                { X: 0, Y: -1 } => new Position(0, 1)
            });
        }

        return decision;
    }
}