using Data;
using Zenject;

namespace Factories
{
    public class StateFactory : IFactory<IMapColorGenerator, State>
    {
        private readonly CoreSettings _coreSettings;

        public StateFactory(CoreSettings coreSettings)
        {
            _coreSettings = coreSettings;
        }

        public State Create(IMapColorGenerator colorGenerator)
        {
            var board = GenerateBoard(_coreSettings.boardSizeX, _coreSettings.boardSizeY);
            var colorsMap = colorGenerator.GenerateMap(_coreSettings.boardSizeX, _coreSettings.boardSizeY);
            return new State(board, colorsMap, 0);
        }
        
        private TileData[,] GenerateBoard(int x, int y)
        {
            TileData[,] board = new TileData[x, y];
            for (int i = 0; i < y; i++)
            {
                for (int j = 0; j < x; j++)
                {
                    board[j, i] = new TileData(j, i);
                }
            }

            return board;
        }
    }
}