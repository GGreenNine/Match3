using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace Game
{
    public interface IGameScore
    {
        public void AddScore(int score);
        public void ClearScore();
        public AsyncReactiveProperty<int> ScoreAddedReactiveProperty { get; }
        public AsyncReactiveProperty<int> ScoreTempReactiveProperty { get; }
        public int Score { get; }
    }
    
    public class GameScoreController : IGameScore, IDisposable
    {
        public AsyncReactiveProperty<int> ScoreTempReactiveProperty => scoreUIReactiveProperty;
        private AsyncReactiveProperty<int> scoreUIReactiveProperty = new(0);
        public AsyncReactiveProperty<int> ScoreAddedReactiveProperty => scoreReactiveProperty;
        private AsyncReactiveProperty<int> scoreReactiveProperty = new(0);
        public int Score => _score;
        private int _score;

        public void AddScore(int score)
        {
            _score += score;
            scoreReactiveProperty.Value = _score;
        }

        public void ClearScore()
        {
            scoreReactiveProperty.Value = 0;
            scoreUIReactiveProperty.Value = 0;
            _score = 0;
        }

        public void Dispose()
        {
            scoreUIReactiveProperty?.Dispose();
            scoreReactiveProperty?.Dispose();
        }
    }
}