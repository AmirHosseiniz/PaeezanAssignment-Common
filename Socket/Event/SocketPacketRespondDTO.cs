using Newtonsoft.Json;

namespace Common.Socket.Event
{
    public class SocketPacketRespondDTO
    {
        public int Type;
        public dynamic? Data { get; set; }
        public string Error { get; set; }

        public string Serialize()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}