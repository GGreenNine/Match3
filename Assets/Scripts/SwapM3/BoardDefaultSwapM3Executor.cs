﻿using System;
using System.Collections.Generic;
using BoardActions;
using Data;
using Exceptions;
using Game;
using UnityEngine;

namespace SwapM3
{
    public class BoardDefaultSwapM3Executor
    {
        private int seed = 23351257;
        public IEnumerable<IBoardAction> SwapExecute(State state, TileData[] tilesForSwap)
        {
            if (!state.IsSwapLegal(tilesForSwap))
            {
                throw new SwapIsNotLegalException();
            }

            var swapAction = new SwapTilesAction();
            swapAction.SwapTiles(ref state, tilesForSwap);
            yield return swapAction;
        
            List<TileData> tilesToProceed = new() {tilesForSwap[0], tilesForSwap[1]};
            HashSet<TileData> processedTiles = new();
        
            while (tilesToProceed.Count > 0)
            {
                var findMatchesAction = new FindMatchesAction();
                tilesToProceed = findMatchesAction.FindMatches(ref state, tilesToProceed, processedTiles);
                if (tilesToProceed.Count <= 0)
                {
                    break;
                }

                var removesTilesAction = new RemoveTilesAction();
                Debug.Log($"Removing {tilesToProceed.Count} tiles");
                removesTilesAction.RemoveMatches(ref state, tilesToProceed);
                yield return removesTilesAction;
                
                var dropTilesAction = new DropTilesAction();
                if (dropTilesAction.DropTiles(ref state))
                {
                    processedTiles.Clear();
                    yield return dropTilesAction;
                }
                
                var addScoreAction = new AddTilesScoreAction();
                addScoreAction.AddScore(ref state, tilesToProceed);
                yield return addScoreAction;
            }

            var _replaceEmptyTilesAction = new ReplaceTilesByLinesDeterministicAction(seed);
            _replaceEmptyTilesAction.ReplaceEmptyTiles(ref state);
            yield return _replaceEmptyTilesAction;
        }
    }
}