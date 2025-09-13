using PaeezanAssignment_Server.Common.Socket.Event;

namespace HokmGG.Backend.Models.SocketDTO;

public class SocketPacketRequestDTO
{
    public ServiceType ServiceType { get; set; }
    public MethodType Method { get; set; }
    public dynamic? Data { get; set; }

}