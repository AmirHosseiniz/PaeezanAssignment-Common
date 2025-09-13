namespace Common.Socket
{
    public class RawRoomNetworkSocketPacket
    {
        public string UserId { get; set; }
        public RoomSocketPacketType Type { get; set; }
        public string Data { get; set; }
    }
}