namespace POrpg.Networking;

public record InputMessage(int PlayerId, string Key);

public record StateMessage(byte[] Dungeon);

public record JoinAckMessage(int PlayerId, byte[] Dungeon);