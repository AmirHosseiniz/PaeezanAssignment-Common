using Common.Socket.Event;
using Newtonsoft.Json;

namespace Common.Socket.Event
{
    public class SocketPacketRequestDTO
    {
        public ServiceType ServiceType { get; set; }
        public MethodType Method { get; set; }
        public dynamic? Data { get; set; }

        public string Serialize()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}