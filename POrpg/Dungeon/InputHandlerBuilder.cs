using POrpg.ConsoleUtils;
using POrpg.InputHandlers;
using POrpg.Items;

namespace POrpg.Dungeon;

public class InputHandlerBuilder
{
    public InputHandler Build(ConsoleView view)
    {
        var handlers = GetHandlers(view);

        handlers.Add(new GuardInputHandler());
        for (var i = 0; i < handlers.Count - 1; i++)
            handlers[i].SetNext(handlers[i + 1]);

        return handlers[0];
    }

    private List<InputHandler> GetHandlers(ConsoleView view)
    {
        if (view.IsChoosingAttack)
            return [new AttackInputHandler(view.Player.Attributes)];

        var handlers = new List<InputHandler>
        {
            new UiInputHandler(),
            new MovementInputHandler(),
            new InventoryInputHandler()
        };

        if (view.CurrentItem != null)
            handlers.Add(new ItemsHandler());
        if (view.CurrentTile.HasManyItems)
            handlers.Add(new CycleItemsHandler());
        if (view.SelectedItem is IUsable)
            handlers.Add(new UsableItemInputHandler());
        if (view.Player.LookingAt?.Enemy != null)
            handlers.Add(new ChooseAttackInputHandler());

        return handlers;
    }
}