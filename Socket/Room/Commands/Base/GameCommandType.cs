namespace PaeezanAssignment_Server.Common.Socket.Room.Commands.Base;

public enum GameCommandType
{
    Init = 0,
    GameSnapShot = 1,
    RequestFrames = 2,
    ReceivedFrames = 3,
    End = 4,
}