using System.Collections.Generic;
using Data;
using UnityEngine;

namespace BoardActions
{
    public class RemoveTilesAction : IBoardAction
    {
        public State ModifiedState => modifiedState;
        private State modifiedState;
        
        public void RemoveMatches(ref State state, IReadOnlyList<TileData> matches)
        {
            foreach (var match in matches)
            {
                state.ColorsMap[match.X, match.Y] = -1;
            }

            modifiedState = state.DeepCopy();
        }
    }


}