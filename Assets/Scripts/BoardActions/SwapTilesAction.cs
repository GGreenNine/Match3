using System.Collections.Generic;
using Data;

namespace BoardActions
{
    public class SwapTilesAction : IBoardAction
    {
        public State ModifiedState => modifiedState;
        private State modifiedState;
        
        public void SwapTiles(ref State state, IReadOnlyList<TileData> tilesToSwap)
        {
            var tileSwap1 = tilesToSwap[0];
            var tileSwap2 = tilesToSwap[1];
            (state.ColorsMap[tileSwap1.X, tileSwap1.Y], state.ColorsMap[tileSwap2.X, tileSwap2.Y]) = 
                (state.ColorsMap[tileSwap2.X, tileSwap2.Y], state.ColorsMap[tileSwap1.X, tileSwap1.Y]);
            modifiedState = state.DeepCopy();
        }
    }
}