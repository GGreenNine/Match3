using System.Collections.Generic;
using System.Linq;
using Data;
using UnityEngine.Profiling;

namespace BoardActions
{
    public class FindMatchesAction : IBoardAction
    {
        public State ModifiedState => modifiedState;
        private State modifiedState;
        
        public List<TileData> FindMatches(ref State state, List<TileData> tileDataList, HashSet<TileData> processedTiles)
        {
            Profiler.BeginSample("FindMatches");
            modifiedState = state;
            Queue<TileData> tilesToProcessQueue = new Queue<TileData>();
            HashSet<TileData> matches = new();
            foreach (var tileData in tileDataList)
            {
                tilesToProcessQueue.Enqueue(tileData);
            }

            while (tilesToProcessQueue.Count > 0)
            {
                var tileToProcess = tilesToProcessQueue.Dequeue();
                var color = state.ColorsMap[tileToProcess.X, tileToProcess.Y];
                if (color == -1 || !CreatesMatch(state.ColorsMap, tileToProcess.X, tileToProcess.Y, color))
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
        
            Profiler.EndSample();
            return matches.ToList();
        }
        
        private bool CreatesMatch(int[,] colorsMap, int x, int y, int color)
        {
            int width = colorsMap.GetLength(0);
            int height = colorsMap.GetLength(1);

            // Horizontal Check (Left, Right, and Middle)
            // Check for three consecutive colors in a row where the current tile is either at the start, middle, or end.
            if (x > 0 && x < width - 1)
            {
                if (colorsMap[x - 1, y] == color && colorsMap[x + 1, y] == color)
                {
                    return true; // Current tile is in the middle of a horizontal match.
                }
            }
            if (x > 1 && colorsMap[x - 1, y] == color && colorsMap[x - 2, y] == color)
            {
                return true; // Current tile is at the right end of a horizontal match.
            }
            if (x < width - 2 && colorsMap[x + 1, y] == color && colorsMap[x + 2, y] == color)
            {
                return true; // Current tile is at the left end of a horizontal match.
            }

            // Vertical Check (Top, Bottom, and Middle)
            // Check for three consecutive colors in a column where the current tile is either at the start, middle, or end.
            if (y > 0 && y < height - 1)
            {
                if (colorsMap[x, y - 1] == color && colorsMap[x, y + 1] == color)
                {
                    return true; // Current tile is in the middle of a vertical match.
                }
            }
            if (y > 1 && colorsMap[x, y - 1] == color && colorsMap[x, y - 2] == color)
            {
                return true; // Current tile is at the bottom end of a vertical match.
            }
            if (y < height - 2 && colorsMap[x, y + 1] == color && colorsMap[x, y + 2] == color)
            {
                return true; // Current tile is at the top end of a vertical match.
            }

            return false; 
        }

        private void EnqueueMatches(List<TileData> foundMatches, HashSet<TileData> matches, Queue<TileData> filesToProcess)
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