using POrpg.InputHandlers;
using POrpg.Items;

namespace POrpg.Dungeon;

public class InputHandlerBuilder
{
    public InputHandler Build(Dungeon dungeon)
    {
        var handlers = GetHandlers(dungeon);
        
        handlers.Add(new GuardInputHandler());
        for (var i = 0; i < handlers.Count - 1; i++)
            handlers[i].SetNext(handlers[i + 1]);
        
        return handlers[0];
    }

    private List<InputHandler> GetHandlers(Dungeon dungeon)
    {
        if (dungeon.IsChoosingAttack)
            return [new AttackInputHandler()];
        
        var handlers = new List<InputHandler>
        {
            new UiInputHandler(),
            new MovementInputHandler(),
            new InventoryInputHandler()
        };

        if (dungeon.CurrentItem != null)
            handlers.Add(new ItemsHandler());
        if (dungeon.CurrentTile.HasManyItems)
            handlers.Add(new CycleItemsHandler());
        if (dungeon.SelectedItem is IUsable)
            handlers.Add(new UsableItemInputHandler());
        if (dungeon.Player.LookingAt?.Enemy != null)
            handlers.Add(new ChooseAttackInputHandler());
        
        return handlers;
    }
}