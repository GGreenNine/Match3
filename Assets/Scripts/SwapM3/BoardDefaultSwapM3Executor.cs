using System.Collections.Generic;
using BoardActions;
using Data;
using Exceptions;
using Game;

namespace SwapM3
{
    public class BoardDefaultSwapM3Executor
    {
        private readonly SwapTilesAction _swapTilesAction = new();
        private readonly FindMatchesAction _findMatchesAction = new();
        private readonly RemoveTilesAction _removeTilesAction = new();
        private readonly DropTilesAction _dropTilesAction = new();
        private readonly ReplaceEmptyTilesAction _replaceEmptyTilesAction = new();
        private readonly AddTilesScoreAction _addTilesScoreAction = new();
        private readonly IGameScore _gameScore;

        public BoardDefaultSwapM3Executor(IGameScore gameScore)
        {
            _gameScore = gameScore;
        }

        public IEnumerable<IBoardAction> SwapExecute(State state, List<TileData> tilesForSwap)
        {
            if (!state.IsSwapLegal(tilesForSwap))
            {
                throw new SwapIsNotLegalException();
            }

            _swapTilesAction.SwapTiles(ref state, tilesForSwap);
        
            List<TileData> tilesToProceed = new() {tilesForSwap[0], tilesForSwap[1]};
            HashSet<TileData> processedTiles = new();
        
            while (tilesToProceed.Count > 0)
            {
                tilesToProceed = _findMatchesAction.FindMatches(ref state, tilesToProceed, processedTiles);
                if (tilesToProceed.Count <= 0)
                {
                    break;
                }

                _removeTilesAction.RemoveMatches(ref state, tilesToProceed);
                yield return _removeTilesAction;
                _addTilesScoreAction.AddScore(ref state, tilesToProceed, _gameScore);
            
                if (_dropTilesAction.DropTiles(ref state))
                {
                    processedTiles.Clear();
                    yield return _dropTilesAction;
                }
            }

            _replaceEmptyTilesAction.ReplaceEmptyTiles(ref state);
            yield return _replaceEmptyTilesAction;
        }
    }
}