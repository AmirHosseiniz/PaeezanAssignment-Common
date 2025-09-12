namespace PaeezanAssignment_Server.Common.Socket.Room.Commands.Base;

public interface IGameCommandHandler
{
    public void OnCommand(GameCommand cmd)
    {
        switch (cmd.Type)
        {
            case GameCommandType.Init:
                OnInit(cmd.GetSubType<InitCMD>());
                break;
            case GameCommandType.GameSnapShot:
                OnGameSnapShot(cmd.GetSubType<GameSnapShotCMD>());
                break;
            case GameCommandType.End:
                OnEndGame(cmd.GetSubType<EndCMD>());
                break;
        }
    }

    public void OnInit(InitCMD cmd);
    public void OnGameSnapShot(GameSnapShotCMD cmd);
    public void OnEndGame(EndCMD cmd);
}