using Data;

namespace BoardActions
{
    public interface IBoardAction
    {
        public State ModifiedState { get; } 
    }
}