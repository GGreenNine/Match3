using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Data;
using GameState;
using UnityEngine;
using Zenject;

namespace Game
{
    public class GameController : IInitializable, IDisposable
    {
        private readonly IGameScore _gameScore;
        private readonly CoreSettings _coreSettings;
        private readonly GameGlobalTimer _globalTimer;
        private readonly GameStateController _gameStateController;

        private IGameMod _gameMod;
        private CancellationTokenSource _cancellationTokenSource = new();

        public GameController(IGameScore gameScore, CoreSettings coreSettings, GameGlobalTimer globalTimer, GameStateController gameStateController)
        {
            _gameScore = gameScore;
            _coreSettings = coreSettings;
            _globalTimer = globalTimer;
            _gameStateController = gameStateController;
        }

        public void Initialize()
        {
            _gameMod = new TimeBasedGameMod(_gameScore, _coreSettings, _globalTimer, _gameStateController);
            _gameStateController.OnStateUpdated += OnStateUpdated;
        }

        private void OnStateUpdated(IGameState state)
        {
            if (state._gameStateType == GameStateType.Gameplay)
            {
                StartGameFlow().Forget();
            }

            if (state._gameStateType == GameStateType.ResultScreen)
            {
                Dispose();
            }
        }

        private async UniTaskVoid StartGameFlow()
        {
            try
            {
                var result = await _gameMod.Execute(_cancellationTokenSource.Token);
                _gameStateController.SetState(new ResultScreenState(result, _gameScore));
                _cancellationTokenSource?.Cancel();
                _cancellationTokenSource?.Dispose();
                _cancellationTokenSource = new CancellationTokenSource();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                Debug.Log("Game stoped");
            }
        }

        public void Dispose()
        {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();
            _cancellationTokenSource = new CancellationTokenSource();
        }
    }
}