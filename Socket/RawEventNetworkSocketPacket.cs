namespace PaeezanAssignment_Server.Common.Socket;

public class RawEventNetworkSocketPacket
{
    public string UserId { get; set; }
    public string Data { get; set; }

    public RawEventNetworkSocketPacket(string userId, string data)
    {
        UserId = userId;
        Data = data;
    }
}