using UnityEngine;

namespace GameState
{
    public class StartScreenState : IGameState
    {
        public GameStateType _gameStateType => GameStateType.StartScren;

        public void EnterState(GameStateController controller)
        {
            Debug.Log("Enter StartScreenState");
        }

        public void UpdateState(GameStateController controller)
        {
            controller.SetState(new GameplayState());
        }

        public void ExitState(GameStateController controller)
        {
            Debug.Log("Exit StartScreenState");
        }
    }
}