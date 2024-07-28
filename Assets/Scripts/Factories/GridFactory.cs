using System;
using System.Collections.Generic;
using Data;
using UnityEngine;
using Zenject;

namespace Factories
{
    public class GridFactory : IFactory<State,Transform, Action<TileControl>, Action<TileControl, Vector3>, Dictionary<TileData, TileControl>>
    {
        private readonly IFactory<TileControl> tileControlFactory;
        private readonly CoreSettings _coreSettings;

        public GridFactory(IFactory<TileControl> tileControlFactory, CoreSettings coreSettings)
        {
            this.tileControlFactory = tileControlFactory;
            _coreSettings = coreSettings;
        }

        public Dictionary<TileData, TileControl> Create(State state, Transform parent, Action<TileControl> onTileClickHandler, Action<TileControl, Vector3> tileDragHanlder)
        {
            Dictionary<TileData, TileControl> _tileControls = new();
            var board = state.Board;
            for (int x = 0; x < board.GetLength(0); x++)
            {
                for (int y = 0; y < board.GetLength(1); y++)
                {
                    var tile = tileControlFactory.Create();
                    tile.transform.SetParent(parent, false);
                    tile.transform.localPosition = new Vector2(x * _coreSettings.tilesSpace, y * _coreSettings.tilesSpace);
                    var tileData = new TileData(x, y);
                    tile.SetupTile(tileData, state.ColorsMap, onTileClickHandler, tileDragHanlder);
                    _tileControls.Add(tileData, tile);
                }
            }

            return _tileControls;
        }

    }
}