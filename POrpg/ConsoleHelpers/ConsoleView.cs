using System.Diagnostics;
using POrpg.Dungeon;
using POrpg.InputHandlers;
using POrpg.Inventory;

namespace POrpg.ConsoleHelpers;

public class ConsoleView
{
    private Dungeon.Dungeon _dungeon;
    private InputHint[] _hints;

    public ConsoleView(Dungeon.Dungeon dungeon, InputHint[] hints)
    {
        _dungeon = dungeon;
        _hints = hints;
    }
    
    public void Draw()
    {
        var sw = Stopwatch.StartNew();
        var console = ConsoleHelper.GetInstance();

        for (var y = 0; y < _dungeon.Height; y++)
        {
            for (var x = 0; x < _dungeon.Width; x++)
            {
                Position pos = (x, y);
                if (pos == _dungeon.Player.Position)
                {
                    console.Write(new StyledText(_dungeon.Player.Symbol, Styles.Player));
                    continue;
                }

                console.Write(_dungeon[pos].Symbol);
            }

            console.WriteLine();
        }

        console.ChangeColumn(1);
        console.WriteLine($"{new StyledText("Turn:", Style.Underline)} {TurnManager.GetInstance().Turn}");
        console.WriteLine();
        DrawStats();
        console.HorizontalDivider();
        DrawInventory();
        console.HorizontalDivider();
        console.ChangeColumn(2);
        if (DrawActiveEffects())
            console.HorizontalDivider();
        if (DrawStandingOn())
            console.HorizontalDivider();
        if (DrawLookingAt())
            console.HorizontalDivider();

        console.ChangeColumn(0);
        console.PrintNotifications();
        console.WriteLine();

        foreach (var hint in _hints.Where(h => (h.Location & UiLocation.Bottom) != 0))
        {
            console.WriteHint(hint);
            console.Write(" ");
        }

        console.WriteLine();

        sw.Stop();
        console.WriteLine(new StyledText($"Frame time: {sw.Elapsed.Milliseconds} ms", Style.Faint));
    }

    private void DrawStats()
    {
        var console = ConsoleHelper.GetInstance();
        console.WriteLine(new StyledText("Player Stats:", Style.Underline));
        foreach (var attribute in _dungeon.Player.Attributes)
        {
            console.Write($"{attribute.Key,-15} ");
            console.WriteLine(new StyledText(attribute.Value.ToString(), Style.Gradient));
        }

        console.WriteLine();
        console.WriteLine($"{"Coins",-15} {new StyledText(_dungeon.Player.Coins.ToString(), Styles.Money)}");
        console.WriteLine($"{"Gold",-15} {new StyledText(_dungeon.Player.Gold.ToString(), Styles.Money)}");
    }

    private void DrawInventory()
    {
        var console = ConsoleHelper.GetInstance();
        console.WriteLine(new StyledText("Inventory:", Style.Underline));

        if (_dungeon.Player.SelectedSlot == new EquipmentSlot(EquipmentSlotType.LeftHand))
        {
            console.Write($"{new StyledText("L", Styles.Player)}. ");
            var leftHand = _dungeon.Player.Inventory[new EquipmentSlot(EquipmentSlotType.LeftHand)];
            if (leftHand != null)
            {
                console.WriteLine(leftHand.Name);
                console.WriteLine(leftHand.Description);
                foreach (var hint in _hints.Where(h => (h.Location & UiLocation.Inventory) != 0))
                    console.WriteHintLine(hint);
            }
            else
            {
                console.WriteLine("Empty");
            }

            console.Write($"{new StyledText("R", Styles.Player, Style.Faint)}. ");
            var rightHand = _dungeon.Player.Inventory.Equipment.RightHand;
            console.WriteLine(rightHand != null ? rightHand.Name : "Empty");
        }
        else if (_dungeon.Player.SelectedSlot == new EquipmentSlot(EquipmentSlotType.RightHand))
        {
            console.Write($"{new StyledText("L", Styles.Player, Style.Faint)}. ");
            var leftHand = _dungeon.Player.Inventory.Equipment.LeftHand;
            console.WriteLine(leftHand != null ? leftHand.Name : "Empty");

            console.Write($"{new StyledText("R", Styles.Player)}. ");
            var rightHand = _dungeon.Player.Inventory.Equipment.RightHand;
            if (rightHand != null)
            {
                console.WriteLine(rightHand.Name);
                console.WriteLine(rightHand.Description);
                foreach (var hint in _hints.Where(h => (h.Location & UiLocation.Inventory) != 0))
                    console.WriteHintLine(hint);
            }
            else
            {
                console.WriteLine("Empty");
            }
        }
        else
        {
            if (_dungeon.Player.Inventory.Equipment.BothHands != null)
            {
                var item = _dungeon.Player.Inventory.Equipment.BothHands;
                if (_dungeon.Player.SelectedSlot == new EquipmentSlot(EquipmentSlotType.BothHands))
                {
                    console.WriteLine(
                        $"{new StyledText("LR", Styles.Player)}. {item.Name}");
                    console.WriteLine(item.Description);
                    foreach (var hint in _hints.Where(h => (h.Location & UiLocation.Inventory) != 0))
                        console.WriteHintLine(hint);
                }
                else
                {
                    console.WriteLine(
                        $"{new StyledText("LR", Styles.Player, Style.Faint)}. {item.Name}");
                }
            }
            else
            {
                console.Write($"{new StyledText("L", Styles.Player, Style.Faint)}. ");
                console.WriteLine(_dungeon.Player.Inventory.Equipment.LeftHand != null
                    ? _dungeon.Player.Inventory.Equipment.LeftHand!.Name
                    : "Empty");
                console.Write($"{new StyledText("R", Styles.Player, Style.Faint)}. ");
                console.WriteLine(_dungeon.Player.Inventory.Equipment.RightHand != null
                    ? _dungeon.Player.Inventory.Equipment.RightHand!.Name
                    : "Empty");
            }
        }

        if (!_dungeon.Player.Inventory.Backpack.IsEmpty)
            console.WriteLine();

        var i = 0;
        foreach (var item in _dungeon.Player.Inventory.Backpack.Items)
        {
            if (_dungeon.Player.SelectedSlot == new BackpackSlot(i))
            {
                console.WriteLine($"{new StyledText((i + 1).ToString(), Styles.Player)}. {item.Name}");
                console.WriteLine(item.Description);
                foreach (var hint in _hints.Where(h => (h.Location & UiLocation.Backpack) != 0))
                    console.WriteHintLine(hint);
            }
            else
            {
                console.WriteLine(
                    $"{new StyledText((i + 1).ToString(), Style.Faint, Styles.Player)}. {item.Name}");
            }

            i++;
        }
    }

    private bool DrawActiveEffects()
    {
        if (_dungeon.Player.Effects.Count == 0) return false;
        var console = ConsoleHelper.GetInstance();
        console.WriteLine($"{new StyledText("Effects:", Style.Underline)}");
        foreach (var effect in _dungeon.Player.Effects.OrderByDescending(e => e.TurnsLeft))
        {
            console.WriteLine(effect.Name);
            console.WriteLine(effect.Description);
        }

        return true;
    }

    private bool DrawStandingOn()
    {
        if (_dungeon.CurrentItem == null) return false;
        var console = ConsoleHelper.GetInstance();

        console.WriteLine(new StyledText("Standing on:", Style.Underline));
        console.WriteLine(_dungeon.CurrentTile.Name);
        console.WriteLine(_dungeon.CurrentTile.Description);
        foreach (var hint in _hints.Where(h => (h.Location & UiLocation.StandingOn) != 0))
            console.WriteHintLine(hint);

        return true;
    }

    private bool DrawLookingAt()
    {
        if (_dungeon.Player.LookingAt == null) return false;
        var console = ConsoleHelper.GetInstance();

        console.WriteLine(new StyledText("Looking at:", Style.Underline));
        console.WriteLine(_dungeon.Player.LookingAt.Name);
        console.WriteLine(_dungeon.Player.LookingAt.Description);
        foreach (var hint in _hints.Where(h => (h.Location & UiLocation.LookingAt) != 0))
            console.WriteHintLine(hint);

        return true;
    }
}