namespace Common.Socket.Room.Commands.Base
{
    public interface IGameCommand
    {
        public T GetSubType<T>() where T : GameCommand;
    }
}