namespace PaeezanAssignment_Server.Common.Socket;

public class RawNetworkSocketPacket
{
    public string UserId { get; set; }
    public RoomSocketPacketType Type { get; set; }
    public string Data { get; set; }
}