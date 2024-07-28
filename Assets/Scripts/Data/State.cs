namespace Data
{
    public readonly struct State
    {
        public TileData[,] Board { get; }
        public int[,] ColorsMap { get; }

        public State(TileData[,] board, int[,] colorsMap)
        {
            Board = board;
            ColorsMap = colorsMap;
        }

        public State DeepCopy()
        {
            var newBoard = new TileData[Board.GetLength(0), Board.GetLength(1)];
            var newColorsMap = new int[ColorsMap.GetLength(0), ColorsMap.GetLength(1)];
        
            for (int i = 0; i < Board.GetLength(0); i++)
            {
                for (int j = 0; j < Board.GetLength(1); j++)
                {
                    newBoard[i, j] = Board[i, j]; // Копируется по значению, так как TileData - структура
                    newColorsMap[i, j] = ColorsMap[i, j]; // Копируется по значению, так как int - структура
                }
            }

            return new State(newBoard, newColorsMap);
        }
    }
}