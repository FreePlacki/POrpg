using POrpg.Commands;

namespace POrpg.Networking;

public interface IMessage;

public sealed record NotificationMessage(string Notification) : IMessage;

public sealed record StateMessage(Dungeon.Dungeon Dungeon) : IMessage;

public sealed record CommandMessage(ICommand Command) : IMessage;

public sealed record JoinMessage(int PlayerId, Dungeon.Dungeon Dungeon, string Instructions) : IMessage;

public sealed record YouDiedMessage : IMessage;