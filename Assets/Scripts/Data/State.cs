using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Data
{
    [Serializable]
    public struct State : IEqualityComparer<State>, IComparable<State>
    {
        public TileData[,] Board { get; }
        public int[,] ColorsMap { get; }

        [JsonIgnore]
        public int Points { get; private set; }
        [JsonIgnore]
        public int EdgeWeight { get; set; }
        [JsonIgnore]
        public int CumulativePoints { get; set; }

        public Guid StateID;

        [JsonIgnore]
        public int Cost
        {
            get
            {
                var maxPoints = Board.GetLength(0) * Board.GetLength(1);
                return Math.Clamp(maxPoints - Points, 0, maxPoints);
            }
        }

        public void SetPoints(int points)
        {
            Points = points;
        }
        [JsonConstructor]
        public State(TileData[,] board, int[,] colorsMap, Guid guid)
        {
            Board = board;
            ColorsMap = colorsMap;
            StateID = guid;
            Points = 0;
            CumulativePoints = 0;
            EdgeWeight = 0;
        }
        public State(TileData[,] board, int[,] colorsMap, int points, int cumulativePoints, int edgeWeight, Guid stateID)
        {
            Board = board;
            ColorsMap = colorsMap;
            Points = points;
            CumulativePoints = cumulativePoints;
            EdgeWeight = edgeWeight;
            StateID = stateID;
        }

        public readonly State DeepCopy()
        {
            var newBoard = new TileData[Board.GetLength(0), Board.GetLength(1)];
            var newColorsMap = new int[ColorsMap.GetLength(0), ColorsMap.GetLength(1)];
        
            for (int i = 0; i < Board.GetLength(0); i++)
            {
                for (int j = 0; j < Board.GetLength(1); j++)
                {
                    newBoard[i, j] = Board[i, j]; 
                    newColorsMap[i, j] = ColorsMap[i, j]; 
                }
            }

            return new State(newBoard, newColorsMap, Points, CumulativePoints, EdgeWeight, StateID);
        }

        public bool Equals(State x, State y)
        {
            if (x.Points != y.Points || x.EdgeWeight != y.EdgeWeight || x.CumulativePoints != y.CumulativePoints)
                return false;

            if (x.Board.GetLength(0) != y.Board.GetLength(0) || x.Board.GetLength(1) != y.Board.GetLength(1))
                return false;

            if (x.ColorsMap.GetLength(0) != y.ColorsMap.GetLength(0) || x.ColorsMap.GetLength(1) != y.ColorsMap.GetLength(1))
                return false;

            for (int i = 0; i < x.Board.GetLength(0); i++)
            {
                for (int j = 0; j < x.Board.GetLength(1); j++)
                {
                    if (!x.Board[i, j].Equals(y.Board[i, j]) || x.ColorsMap[i, j] != y.ColorsMap[i, j])
                        return false;
                }
            }

            return true;
        }

        public int GetHashCode(State obj)
        {
            int hash = 17;

            hash = hash * 31 + obj.Points.GetHashCode();
            hash = hash * 31 + obj.EdgeWeight.GetHashCode();
            hash = hash * 31 + obj.CumulativePoints.GetHashCode();

            foreach (var tile in obj.Board)
            {
                hash = hash * 31 + tile.GetHashCode();
            }

            foreach (var color in obj.ColorsMap)
            {
                hash = hash * 31 + color.GetHashCode();
            }

            return hash;
        }

        public int CompareTo(State other)
        {
            return EdgeWeight.CompareTo(other.EdgeWeight);
        }
    }
}