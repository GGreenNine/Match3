using System;
using System.Collections.Generic;
using BoardActions;
using Data;

public static class StateExtensions
{
    public static bool IsSwapLegal(this State state, List<TileData> tilesForSwap)
    {
        var swapAction = new SwapTilesAction();
        var findMatchesAction = new FindMatchesAction();
        
        if (tilesForSwap.Count != 2 || !tilesForSwap[0].TileIsAdjacentTo(tilesForSwap[1]))
        {
            return false;
        }

        var tempState =  state.DeepCopy();
        var processedTiles = new HashSet<TileData>();
        swapAction.SwapTiles(ref tempState, new[]{tilesForSwap[0], tilesForSwap[1]});
        var legalActions = findMatchesAction.FindMatches(ref tempState, new List<TileData>() {tilesForSwap[0], tilesForSwap[1]}, processedTiles);
        return legalActions.Count > 0;
    }
    
    public static bool ColorsMatch(this State state, TileData current, TileData target)
    {
        if (current.X > state.ColorsMap.GetLength(0) || current.Y > state.ColorsMap.GetLength(1) ||
            target.X > state.ColorsMap.GetLength(0) || target.Y > state.ColorsMap.GetLength(1))
        {
            throw new Exception("Coordinates are outside of the colors map");
        }

        var currentColor = state.ColorsMap[current.X, current.Y];
        var targetColor = state.ColorsMap[target.X, target.Y];

        if (currentColor == -1 || targetColor == -1)
        {
            return false;
        }
        if (currentColor == targetColor)
        {
            return true;
        }

        return false;
    }
    
}