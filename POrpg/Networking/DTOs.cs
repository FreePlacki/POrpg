using POrpg.Commands;

namespace POrpg.Networking;

public interface IMessage;

public record NotificationMessage(string Notification) : IMessage;

public record StateMessage(Dungeon.Dungeon Dungeon) : IMessage;

public record CommandMessage(ICommand Command) : IMessage;

public record JoinMessage(int PlayerId, Dungeon.Dungeon Dungeon, string Instructions) : IMessage;

public record YouDiedMessage : IMessage;