using UnityEngine;

namespace GameState
{
    public class GameplayState : IGameState
    {
        public GameStateType _gameStateType => GameStateType.Gameplay;

        public void EnterState(GameStateController controller)
        {
            Debug.Log("EnterGameplayState");
        }

        public void UpdateState(GameStateController controller) { }

        public void ExitState(GameStateController controller)
        {
            Debug.Log("Exit gameplay state");
        }
    }
}