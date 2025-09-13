namespace Common.Socket.Room.Commands.Base
{
    public class GameCommand : IGameCommand
    {
        public GameCommandType Type { get; set; }
        public string UserId { get; set; }
        public int PlayerIndex { get; set; }

        public T GetSubType<T>() where T : GameCommand
        {
            return this as T;
        }
    }
}