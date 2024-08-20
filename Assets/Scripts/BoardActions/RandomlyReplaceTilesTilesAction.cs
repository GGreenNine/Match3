using Data;

namespace BoardActions
{
    public class RandomlyReplaceTilesTilesAction : IBoardAction
    {
        public State ModifiedState => modifiedState;
        private State modifiedState;
        
        public void ReplaceEmptyTiles(ref State state)
        {
            for (int i = 0; i < state.Board.GetLength(1); i++)
            {
                for (int j = 0; j < state.Board.GetLength(0); j++)
                {
                    var tile = state.Board[i, j];
                    var isTileEmpty = state.ColorsMap[tile.X, tile.Y] == -1;
                    if (isTileEmpty)
                    {
                        // state.ColorsMap[i, j] = ColorsMapRandomGeneration.GetRandomColor(1);
                    }
                }
            }

            modifiedState = state.DeepCopy();
        }
    }
}