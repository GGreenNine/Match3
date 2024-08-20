using System;
using System.Collections.Generic;
using Data;

public static class TileExtensions
{
    public static bool TileIsAdjacentTo(this TileData tile1, TileData tile2)
    {
        int x1 = tile1.X;
        int y1 = tile1.Y;
        int x2 = tile2.X;
        int y2 = tile2.Y;

        return (x1 == x2 && Math.Abs(y1 - y2) == 1) || (y1 == y2 && Math.Abs(x1 - x2) == 1);
    }

    public static List<TileData> FindAdjacentTiles(this TileData tile, in State state)
    {
        List<TileData> adjacentTiles = new List<TileData>();

        int x = tile.X;
        int y = tile.Y;

        int[,] offsets = { 
            { 0, 1 }, 
            { 0, -1 },
            { 1, 0 },  
            { -1, 0 }  
        };

        for (int i = 0; i < offsets.GetLength(0); i++)
        {
            int newX = x + offsets[i, 0];
            int newY = y + offsets[i, 1];

            if (newX >= 0 && newX < state.Board.GetLength(0) && newY >= 0 && newY < state.Board.GetLength(1))
            {
                adjacentTiles.Add(state.Board[newX, newY]);
            }
        }

        return adjacentTiles;
    }
}