using BoardActions;
using Data;
using UnityEngine;

public class ColorsMapGeneration : IMapColorGenerator
{
    public int[,] GenerateMap(ref State state, int seed)
    {
        ReplaceTilesByLinesDeterministicAction generateTilesAction = new(seed);
        generateTilesAction.ReplaceEmptyTiles(ref state);
        return generateTilesAction.ModifiedState.ColorsMap;
    }
}

public interface IMapColorGenerator
{
    public int[,] GenerateMap(ref State state, int seed);
}