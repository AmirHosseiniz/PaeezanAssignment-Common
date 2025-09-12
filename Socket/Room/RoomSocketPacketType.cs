namespace PaeezanAssignment_Server.Common.Socket;

public enum RoomSocketPacketType
{
    PI, // ping
    PO, // pong
    C, // connect 
    D, // disconnect
    M  // message
}