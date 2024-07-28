using System;

namespace Data
{
    public readonly struct TileData : IEquatable<TileData>
    {
        public int X { get; }
        public int Y { get; }

        public TileData(int x, int y)
        {
            X = x;
            Y = y;
        }

        public bool Equals(TileData other)
        {
            return X == other.X && Y == other.Y;
        }

        public override bool Equals(object obj)
        {
            return obj is TileData other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y);
        }
    }
}
