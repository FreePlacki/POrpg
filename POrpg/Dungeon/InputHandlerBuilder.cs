using POrpg.InputHandlers;

namespace POrpg.Dungeon;

public class InputHandlerBuilder : IDungeonBuilder<InputHandler>
{
    private bool _hasItems;
    private bool _hasEnemies;
    private bool _hasUsableItems;

    public IDungeonBuilder<InputHandler> AddRandomChambers(int numChambers) => this;

    public IDungeonBuilder<InputHandler> AddCentralRoom() => this;

    public IDungeonBuilder<InputHandler> AddRandomPaths() => this;

    public IDungeonBuilder<InputHandler> AddUnusableItems(double probability = 0.07)
    {
        _hasItems = true;
        return this;
    }

    public IDungeonBuilder<InputHandler> AddModifiedUnusableItems(double probability = 0.07, int maxEffects = 3)
    {
        _hasItems = true;
        return this;
    }

    public IDungeonBuilder<InputHandler> AddWeapons(double probability = 0.07)
    {
        _hasItems = true;
        return this;
    }

    public IDungeonBuilder<InputHandler> AddModifiedWeapons(double probability = 0.1, int maxEffects = 3)
    {
        _hasItems = true;
        return this;
    }

    public IDungeonBuilder<InputHandler> AddPotions(double probability = 0.15)
    {
        _hasItems = true;
        _hasUsableItems = true;
        return this;
    }

    public IDungeonBuilder<InputHandler> AddEnemies(double probability = 0.15)
    {
        _hasEnemies = true;
        return this;
    }

    public IDungeonBuilder<InputHandler> AddMoney(double probability = 0.15)
    {
        _hasItems = true;
        return this;
    }

    public InputHandler Build()
    {
        var handlers = new List<InputHandler>
        {
            new UiInputHandler(),
            new MovementInputHandler()
        };

        if (_hasItems || _hasEnemies)
            handlers.Add(new CycleItemsHandler());
        if (_hasItems)
            handlers.Add(new InventoryInputHandler());
        if (_hasUsableItems)
            handlers.Add(new UsableItemInputHandler());
        handlers.Add(new GuardInputHandler());

        for (var i = 0; i < handlers.Count - 1; i++)
            handlers[i].SetNext(handlers[i + 1]);
        
        return handlers[0];
    }
}