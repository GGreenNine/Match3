using Data;

namespace BoardActions
{
    public class GenerateColorsAction
    {
        public int[,] GenerateColorsBoard(int x, int y, int seed)
        {
            int[,] colorsMap = CreateEmptyBoard(x, y);
            ReplaceTilesByLinesDtAction replaceTilesAction = new(seed);
            replaceTilesAction.ReplaceEmptyTiles(colorsMap);
            return colorsMap;
        }

        private int[,] CreateEmptyBoard(int x, int y)
        {
            int[,] emptyBoard = new int[x, y];
            for (int i = 0; i < x; i++)
            {
                for (int j = 0; j < y; j++)
                {
                    emptyBoard[i, j] = -1;
                }
            }

            return emptyBoard;
        }
        
    }
}