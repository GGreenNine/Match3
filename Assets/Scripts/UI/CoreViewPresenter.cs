using System;
using System.Threading;
using Cysharp.Threading.Tasks.Linq;
using Data;
using Game;
using GameState;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace UI
{
    public class CoreViewPresenter : MonoBehaviour
    {
        [SerializeField] private GameObject _resultScreen;
        [SerializeField] private GameObject _startScreen;
        [SerializeField] private TextMeshProUGUI _resultText;
        [SerializeField] private Button _playButton;
        [SerializeField] private Button _exitButton;
        [SerializeField] private TextMeshProUGUI _totalScore;
        [SerializeField] private TextMeshProUGUI _timeLeft;
        [SerializeField] private GameObject _interactionBlockScreen;
        
        private IGameStateController _gameStateController;
        private IGameScore _gameScore;
        private CoreSettings _coreSettings;
        private GameGlobalTimer _globalTimer;
        private readonly CancellationTokenSource _cancellationTokenSource = new();
        
        [Inject]
        private void SetDependencies(IGameStateController gameStateController, IGameScore gameScore,
            GameGlobalTimer globalTimer, CoreSettings coreSettings)
        {
            _gameStateController = gameStateController;
            _gameScore = gameScore;
            _globalTimer = globalTimer;
            _coreSettings = coreSettings;
        }

        private void Awake()
        {
            _gameStateController.OnStateUpdated += OnStateUpdated;
            _playButton.onClick.AddListener(PlayButtonClicked);
            _exitButton.onClick.AddListener(ExitButtonClicked);
            _gameScore.ScoreAddedReactiveProperty.Subscribe(OnScoreUpdate, _cancellationTokenSource.Token);
        }

        private void Update()
        {
            UpdateTimeLeft();
        }

        private void UpdateTimeLeft()
        {
            var timeLeft = (int)(_coreSettings.targetTime - _globalTimer.TimeElapsed.TotalSeconds);
            _timeLeft.text = timeLeft <= 0 ? "0" : timeLeft.ToString();
        }

        private void OnScoreUpdate(int score)
        {
            _totalScore.text = score.ToString();
        }

        private void ExitButtonClicked()
        {
            _gameStateController.Update();
        }

        private void PlayButtonClicked()
        {
            _gameStateController.Update();
        }

        private void OnStateUpdated(IGameState newState)
        {
            switch (newState._gameStateType)
            {
                case GameStateType.StartScren:
                    _resultScreen.SetActive(false);
                    _startScreen.SetActive(true);
                    _interactionBlockScreen.gameObject.SetActive(false);
                    break;
                case GameStateType.ResultScreen:
                    UpdateResultText();
                    _resultScreen.SetActive(true);
                    _startScreen.SetActive(false);
                    _interactionBlockScreen.gameObject.SetActive(false);
                    break;
                case GameStateType.Gameplay:
                    _interactionBlockScreen.gameObject.SetActive(false);
                    _resultScreen.SetActive(false);
                    _startScreen.SetActive(false);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void UpdateResultText()
        {
            if (_gameStateController.CurrentState is ResultScreenState resultState)
            {
                _resultText.text = resultState.Result 
                    ? $"You've completed with score {_gameScore.Score.ToString()}" 
                    : $"You have failed the level";
            }
        }

        private void OnDestroy()
        {
            _gameStateController.OnStateUpdated -= OnStateUpdated;
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();
            _playButton.onClick.RemoveAllListeners();
            _exitButton.onClick.RemoveAllListeners();
        }
    }
}