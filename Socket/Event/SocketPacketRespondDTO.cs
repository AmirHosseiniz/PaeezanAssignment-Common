using Newtonsoft.Json;

namespace HokmGG.Backend.Models.SocketDTO;

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