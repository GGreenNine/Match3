namespace GameState
{
    public interface IGameState
    {
        GameStateType _gameStateType { get; }
        void EnterState(GameStateController controller);
        void UpdateState(GameStateController controller);
        void ExitState(GameStateController controller);
    }

    public interface IResultScreenState : IGameState
    {
        bool Result { get; }
    }
}