using System;
using System.Diagnostics;
using GameState;
using Zenject;

namespace Game
{
    public class GameGlobalTimer : IInitializable
    {
        private readonly IGameStateController _gameStateController;
        private readonly Stopwatch _stopwatch = new();
        public TimeSpan TimeElapsed => _stopwatch.Elapsed;
        
        public GameGlobalTimer(IGameStateController gameStateController)
        {
            _gameStateController = gameStateController;
        }

        public void StopTime()
        {
            _stopwatch.Stop();
        }

        public void Continue()
        {
            _stopwatch.Start();
        }

        public void Initialize()
        {
            _gameStateController.OnStateUpdated += OnStateUpdated;
        }

        private void OnStateUpdated(IGameState state)
        {
            if (state._gameStateType == GameStateType.Gameplay)
            {
                _stopwatch.Restart();
            }

            if (state._gameStateType == GameStateType.ResultScreen)
            {
                _stopwatch.Stop();
            }
        }
    }
}