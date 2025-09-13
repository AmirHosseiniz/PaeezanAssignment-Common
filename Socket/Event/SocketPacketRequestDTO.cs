using Common.Socket.Event;

namespace Common.Socket.Event
{
    public class SocketPacketRequestDTO
    {
        public ServiceType ServiceType { get; set; }
        public MethodType Method { get; set; }
        public dynamic? Data { get; set; }
    }
}