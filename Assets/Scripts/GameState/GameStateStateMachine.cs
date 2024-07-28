using System;
using IInitializable = Zenject.IInitializable;

namespace GameState
{
    public interface IGameStateController
    {
        public void SetState(IGameState newState);
        public void Update();
        public Action<IGameState> OnStateUpdated { get; set; }
        public IGameState CurrentState { get; }
    }

    public class GameStateController : IGameStateController, IInitializable
    {
        public IGameState CurrentState => currentState;
        private IGameState currentState;
        public Action<IGameState> OnStateUpdated { get; set; }

        public void Initialize()
        {
            SetState(new StartScreenState());
        }
        public void SetState(IGameState newState)
        {
            if (currentState != null)
            {
                currentState.ExitState(this);
            }

            currentState = newState;
            currentState.EnterState(this);
            OnStateUpdated?.Invoke(newState);
        }

        public void Update()
        {
            if (currentState != null)
            {
                currentState.UpdateState(this);
            }
        }

    }
}