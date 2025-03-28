using POrpg.InputHandlers;

namespace POrpg.Dungeon;

public class InputHandlerBuilder : IDungeonBuilder<InputHandler>
{
    private bool _hasItems;
    private bool _hasEnemies;
    
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
        var inputHandler = new MovementInputHandler();
        if (_hasItems || _hasEnemies)
            inputHandler.SetNext(new CycleItemsHandler());
        if (_hasItems)
            inputHandler.SetNext(new InventoryInputHandler());
        inputHandler.SetNext(new GuardInputHandler());
        
        return inputHandler;
    }
}