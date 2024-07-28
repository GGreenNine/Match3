using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using Data;
using GameState;
using UnityEngine;

namespace Game
{
    public interface IGameMod
    {
        //executes game mode, bool indicates if the game mode was fulfilled  
        public UniTask<bool> Execute(CancellationToken ct);
    }

    public class TimeBasedGameMod : IGameMod
    {
        private readonly IGameScore _gameScore;
        private readonly CoreSettings _coreSettings;
        private readonly GameGlobalTimer _gameGlobalTimer;
        private readonly IGameStateController _gameStateController;

        public TimeBasedGameMod(IGameScore gameScore, CoreSettings coreSettings, GameGlobalTimer gameGlobalTimer, IGameStateController gameStateController)
        {
            _gameScore = gameScore;
            _coreSettings = coreSettings;
            _gameGlobalTimer = gameGlobalTimer;
            _gameStateController = gameStateController;
        }

        public async UniTask<bool> Execute(CancellationToken ct)
        {
            try
            {
                using var combinedToken = CancellationTokenSource.CreateLinkedTokenSource(ct);
                var scoreRequirement = _gameScore.ScoreAddedReactiveProperty.Where(i => i >= _coreSettings.targetScore).FirstOrDefaultAsync(combinedToken.Token).AsUniTask();
                var timeRequirement = UniTask.WaitUntil(() => _gameGlobalTimer.TimeElapsed.TotalSeconds >= _coreSettings.targetTime, cancellationToken: combinedToken.Token);
                var result = await UniTask.WhenAny(scoreRequirement, timeRequirement);
                switch (result)
                {
                    case 0: { return true; }
                    case 1: { return false; }
                    default:
                        Debug.Log($"{result}");
                        return false;
                }
            }
            catch (Exception e)
            {
                if (e is OperationCanceledException)
                {
                    Debug.Log("Time mode cancelled");
                    return false;
                }
                
                Debug.LogException(e);
                return false;
            }
        }
    }
}