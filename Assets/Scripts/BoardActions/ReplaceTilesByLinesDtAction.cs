using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using Random = System.Random;
using State = Data.State;

namespace BoardActions
{
    public class ReplaceTilesByLinesDtAction : IBoardAction
    {
        public State ModifiedState => modifiedState;
        private State modifiedState;

        private readonly int[] AvailableColors;
        private readonly Random random;

        public ReplaceTilesByLinesDtAction(int seed)
        {
            AvailableColors = ColorsMapDefinitions._colorDefinitions.Where(pair => pair.Key != -1).Select(pair => pair.Key).ToArray();
            random = new Random(seed); 
        }

        public void ReplaceEmptyTiles(ref State state)
        {
            ReplaceEmptyTiles(state.ColorsMap);
            modifiedState = state.DeepCopy(); 
        }
        
        public void ReplaceEmptyTiles(int[,] colorsMap)
        {
            for (int x = 0; x < colorsMap.GetLength(0); x++) 
            {
                for (int y = 0; y < colorsMap.GetLength(1); y++) 
                {
                    var isTileEmpty = colorsMap[x, y] == -1;
                    if (isTileEmpty)
                    {
                        int newColor = GetRandomNonMatchingColor(colorsMap, x, y);
                        colorsMap[x, y] = newColor;
                    }
                }
            }
        }

        private int GetRandomNonMatchingColor(int[,] colorsMap, int x, int y)
        {
            var shuffledColors = AvailableColors.OrderBy(c => random.Next()).ToArray();

            foreach (var color in shuffledColors)
            {
                if (!CreatesMatch(colorsMap, x, y, color))
                {
                    return color;
                }
            }
        
            Debug.Log("Can't find a color for the tile");
            return shuffledColors[0];
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
    }
}