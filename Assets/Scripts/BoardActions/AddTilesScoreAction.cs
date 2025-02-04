﻿using System.Collections.Generic;
using Data;
using Game;

namespace BoardActions
{
    public class AddTilesScoreAction : IBoardAction
    {
        public State ModifiedState => modifiedState;
        private State modifiedState;

        public void AddScore(ref State state, IReadOnlyList<TileData> matches)
        {
            state.SetPoints(matches.Count);
            modifiedState = state.DeepCopy();
        }
    }
}