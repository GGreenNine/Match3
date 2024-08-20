using System;
using BoardActions;
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
            int seed = 2355213;
            var generateColorsAction = new GenerateColorsAction();
            var board = GenerateBoard(_coreSettings.boardSizeX, _coreSettings.boardSizeY);
            var colorsMap = generateColorsAction.GenerateColorsBoard(_coreSettings.boardSizeX, _coreSettings.boardSizeY, seed);
            return new State(board, colorsMap, 0, 0, 0, Guid.NewGuid());
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