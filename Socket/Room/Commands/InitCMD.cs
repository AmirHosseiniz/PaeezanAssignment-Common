using Common.Socket.Room.Commands.Base;

namespace Common.Socket.Room.Commands
{
    public class InitCMD : GameCommand
    {
        public readonly string S;

        public InitCMD(string s)
        {
            S = s;
        }
    }
}