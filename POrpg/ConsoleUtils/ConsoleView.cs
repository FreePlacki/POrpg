using POrpg.Dungeon;
using POrpg.InputHandlers;
using POrpg.Inventory;
using POrpg.Items;

namespace POrpg.ConsoleUtils;

public class ConsoleView
{
    public Dungeon.Dungeon Dungeon;

    public bool IsChoosingAttack;
    public bool ShouldQuit;
    public int PlayerId { get; }
    public Player Player => Dungeon.Players[PlayerId];
    public Tile CurrentTile => Dungeon[Player.Position];
    public Item? CurrentItem => CurrentTile.CurrentItem;
    public Item? SelectedItem => Player.SelectedSlot != null ? Player.Inventory[Player.SelectedSlot] : null;

    private InputHint[] _hints = [];

    public ConsoleView(Dungeon.Dungeon dungeon, int playerId)
    {
        Dungeon = dungeon;
        PlayerId = playerId;
    }

    public void SetHints(InputHint[] hints) => _hints = hints;

    public void Draw()
    {
        var console = ConsoleHelper.GetInstance();

        for (var y = 0; y < Dungeon.Height; y++)
        {
            for (var x = 0; x < Dungeon.Width; x++)
            {
                Position pos = (x, y);
                var player = Dungeon.Players.Values.FirstOrDefault(p => p.Position == pos);
                if (player != null)
                {
                    console.Write(new StyledText(player.Symbol, Styles.Player));
                    continue;
                }

                console.Write(Dungeon[pos].Symbol);
            }

            console.WriteLine();
        }

        console.WriteLine();
        console.PrintNotifications();
        console.WriteLine();

        foreach (var hint in _hints.Where(h => (h.Location & UiLocation.Bottom) != 0))
        {
            console.WriteHint(hint);
            console.Write(" ");
        }

        for (var i = 0; i < 5; i++) console.WriteLine();

        console.ChangeColumn(1);
        DrawTurn();
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
    }

    private void DrawTurn()
    {
        var console = ConsoleHelper.GetInstance();
        var turn = Dungeon.TurnManager.Turn;
        var player = Dungeon.TurnManager.CurrentlyPlaying.ToString();
        console.WriteLine(
            $"{new StyledText("Turn:", Style.Underline)} {turn} (Player {new StyledText(player, Styles.Player)})");
    }

    private void DrawStats()
    {
        var console = ConsoleHelper.GetInstance();
        console.WriteLine(new StyledText($"Player {new StyledText(PlayerId.ToString(), Styles.Player)} Stats:",
            Style.Underline));
        foreach (var attribute in Player.Attributes.attributes)
        {
            console.Write($"{attribute.Key,-15} ");
            console.WriteLine(new StyledText(attribute.Value.ToString(), Style.Gradient));
        }

        console.WriteLine();
        console.WriteLine($"{"Coins",-15} {new StyledText(Player.Coins.ToString(), Styles.Money)}");
        console.WriteLine($"{"Gold",-15} {new StyledText(Player.Gold.ToString(), Styles.Money)}");
    }

    private void DrawInventory()
    {
        var console = ConsoleHelper.GetInstance();
        console.WriteLine(new StyledText("Inventory:", Style.Underline));

        if (Player.SelectedSlot == new EquipmentSlot(EquipmentSlotType.LeftHand))
        {
            console.Write($"{new StyledText("L", Styles.Player)}. ");
            var leftHand = Player.Inventory[new EquipmentSlot(EquipmentSlotType.LeftHand)];
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
            var rightHand = Player.Inventory.Equipment.RightHand;
            console.WriteLine(rightHand != null ? rightHand.Name : "Empty");
        }
        else if (Player.SelectedSlot == new EquipmentSlot(EquipmentSlotType.RightHand))
        {
            console.Write($"{new StyledText("L", Styles.Player, Style.Faint)}. ");
            var leftHand = Player.Inventory.Equipment.LeftHand;
            console.WriteLine(leftHand != null ? leftHand.Name : "Empty");

            console.Write($"{new StyledText("R", Styles.Player)}. ");
            var rightHand = Player.Inventory.Equipment.RightHand;
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
            if (Player.Inventory.Equipment.BothHands != null)
            {
                var item = Player.Inventory.Equipment.BothHands;
                if (Player.SelectedSlot == new EquipmentSlot(EquipmentSlotType.BothHands))
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
                console.WriteLine(Player.Inventory.Equipment.LeftHand != null
                    ? Player.Inventory.Equipment.LeftHand!.Name
                    : "Empty");
                console.Write($"{new StyledText("R", Styles.Player, Style.Faint)}. ");
                console.WriteLine(Player.Inventory.Equipment.RightHand != null
                    ? Player.Inventory.Equipment.RightHand!.Name
                    : "Empty");
            }
        }

        if (!Player.Inventory.Backpack.IsEmpty)
            console.WriteLine();

        var i = 0;
        foreach (var item in Player.Inventory.Backpack.Items)
        {
            if (Player.SelectedSlot == new BackpackSlot(i))
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
        if (Player.Effects.Count == 0) return false;
        var console = ConsoleHelper.GetInstance();
        console.WriteLine($"{new StyledText("Effects:", Style.Underline)}");
        foreach (var effect in Player.Effects.OrderByDescending(e => e.Duration))
        {
            console.WriteLine(effect.Name);
            console.WriteLine(effect.Description);
        }

        return true;
    }

    private bool DrawStandingOn()
    {
        if (CurrentItem == null) return false;
        var console = ConsoleHelper.GetInstance();

        console.WriteLine(new StyledText("Standing on:", Style.Underline));
        console.WriteLine(CurrentTile.Name);
        console.WriteLine(CurrentTile.Description);
        foreach (var hint in _hints.Where(h => (h.Location & UiLocation.StandingOn) != 0))
            console.WriteHintLine(hint);

        return true;
    }

    private bool DrawLookingAt()
    {
        if (Player.LookingAt == null) return false;
        var console = ConsoleHelper.GetInstance();

        console.WriteLine(new StyledText("Looking at:", Style.Underline));
        console.WriteLine(Player.LookingAt.Name);
        console.WriteLine(Player.LookingAt.Description);
        foreach (var hint in _hints.Where(h => (h.Location & UiLocation.LookingAt) != 0))
            console.WriteHintLine(hint);

        return true;
    }
}