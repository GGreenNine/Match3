using System.Collections.Generic;
using Data;

namespace BoardActions
{
    public class DropTilesAction : IBoardAction
    {
        public State ModifiedState => modifiedState;
        private State modifiedState;
        
        public bool DropTiles(ref State state)
        {
            bool tilesDropped = false;
            for (int x = 0; x < state.Board.GetLength(0); x++)
            {
                Queue<int> emptyTilesQueue = new();
                for (int y = 0 ; y < state.Board.GetLength(1); y++)
                {
                    var currentTileColor = state.ColorsMap[x, y];
                    if (currentTileColor == -1)
                    {
                        emptyTilesQueue.Enqueue(y);
                    }
                    else if (emptyTilesQueue.Count > 0)
                    {
                        var emptyTileIndex = emptyTilesQueue.Dequeue();
                        state.ColorsMap[x, emptyTileIndex] = state.ColorsMap[x, y];
                        state.ColorsMap[x, y] = -1;
                        emptyTilesQueue.Enqueue(y);
                        tilesDropped = true;
                    }
                }
            }

            modifiedState = state.DeepCopy();
            return tilesDropped;
        }

    }
}