using POrpg.Dungeon;
using POrpg.Enemies.Decisions;

namespace POrpg.Enemies.Behaviours;

public class AggressiveBehaviour : IBehaviour
{
    private Position? _lastSeenTarget;

    public Decision ComputeAction(Position position, Dungeon.Dungeon dungeon)
    {
        var target = dungeon.Players.Values
            .Where(p => dungeon.CanSee(position, p.Position))
            .MinBy(p => position.Distance(p.Position))
            ?.Position;
        if (target == null)
        {
            if (_lastSeenTarget == null)
                return new StayDecision();
            target = _lastSeenTarget;
        }

        _lastSeenTarget = target;

        if (position.Distance(target.Value) == 1)
        {
            var playerToAttack = dungeon.Players
                .Where(p => p.Value.Position == target.Value)
                .Select(p => p.Key).ToArray();
            if (playerToAttack.Length != 0)
                return new AttackDecision(position, playerToAttack.First());
        }

        return new MoveDecision(position, target.Value);
    }
}