namespace Common.Socket
{
    public class RawEventNetworkSocketPacket
    {
        public string UserId { get; set; }
        public dynamic? Data { get; set; }

        public RawEventNetworkSocketPacket(string userId, dynamic? data)
        {
            UserId = userId;
            Data = data;
        }
    }
}