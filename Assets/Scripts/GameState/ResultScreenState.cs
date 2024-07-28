using Game;
using UnityEngine;

namespace GameState
{
    public class ResultScreenState : IResultScreenState
    {
        private readonly bool _result;
        private readonly IGameScore _gameScore;
        public bool Result => _result;

        public ResultScreenState(bool result, IGameScore gameScore)
        {
            _result = result;
            _gameScore = gameScore;
        }

        public GameStateType _gameStateType => GameStateType.ResultScreen;

        public void EnterState(GameStateController controller)
        {
            Debug.Log("Enter ResultScreenState");
        }

        public void UpdateState(GameStateController controller)
        {
            controller.SetState(new StartScreenState());
        }

        public void ExitState(GameStateController controller)
        {
            _gameScore.ClearScore();
            Debug.Log("Exit ResultScreenState");
        }

    }
}