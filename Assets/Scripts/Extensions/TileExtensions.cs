using System;
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
}