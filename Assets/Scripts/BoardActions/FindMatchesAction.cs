using System.Collections.Generic;
using System.Linq;
using Data;

namespace BoardActions
{
    public class FindMatchesAction : IBoardAction
    {
        public State ModifiedState => modifiedState;
        private State modifiedState;
        
        public List<TileData> FindMatches(ref State state, List<TileData> tileDataList, HashSet<TileData> processedTiles)
        {
            modifiedState = state;
            Queue<TileData> tilesToProcessQueue = new Queue<TileData>();
            List<TileData> matches = new();
            foreach (var tileData in tileDataList)
            {
                tilesToProcessQueue.Enqueue(tileData);
            }

            while (tilesToProcessQueue.Count > 0)
            {
                var tileToProcess = tilesToProcessQueue.Dequeue();
                if (state.ColorsMap[tileToProcess.X, tileToProcess.Y] == -1)
                {
                    processedTiles.Add(tileToProcess);
                    continue;
                }

                if (!processedTiles.Add(tileToProcess))
                {
                    continue;
                }

                var verticalMatches = FindVerticalMatches(state, tileToProcess);
                var horizontalMatches = FindHorizontalMatches(state, tileToProcess);

                EnqueueMatches(horizontalMatches, matches, tilesToProcessQueue);
                EnqueueMatches(verticalMatches, matches, tilesToProcessQueue);
            }
        
            if (matches.Count < 3)
            {
                return new List<TileData>();
            }
        
            return matches;
        }

        private void EnqueueMatches(List<TileData> foundMatches, List<TileData> matches, Queue<TileData> filesToProcess)
        {
            foreach (var match in foundMatches)
            {
                matches.Add(match);
                filesToProcess.Enqueue(match);
            }
        }
        private List<TileData> FindHorizontalMatches(State state, TileData tile)
        {
            var matches = new HashSet<TileData> { tile };

            for (int i = tile.X - 1; i >= 0; i--)
            {
                var neighborTile = state.Board[i, tile.Y];
                if (state.ColorsMatch(tile, neighborTile))
                {
                    matches.Add(neighborTile);
                }
                else
                {
                    break;
                }
            }
            for (int i = tile.X + 1; i < state.Board.GetLength(0); i++)
            {
                var neighborTile = state.Board[i, tile.Y];
                if (state.ColorsMatch(tile, neighborTile))
                {
                    matches.Add(neighborTile);
                }
                else
                {
                    break;
                }
            }

            if (matches.Count < 3)
            {
                return new List<TileData>();
            }

            return matches.ToList();
        }

        private List<TileData> FindVerticalMatches(State state, TileData tile)
        {
            var matches = new HashSet<TileData> { tile };

            for (int i = tile.Y - 1; i >= 0; i--)
            {
                var neighborTile = state.Board[tile.X, i];
                if (state.ColorsMatch(tile, neighborTile))
                {
                    matches.Add(neighborTile);
                }
                else
                {
                    break;
                }
            }
            for (int i = tile.Y + 1; i < state.Board.GetLength(1); i++)
            {
                var neighborTile = state.Board[tile.X, i];
                if (state.ColorsMatch(tile, neighborTile))
                {
                    matches.Add(neighborTile);
                }
                else
                {
                    break;
                }
            }

            if (matches.Count < 3)
            {
                return new List<TileData>();
            }

            return matches.ToList();
        }
    }
}