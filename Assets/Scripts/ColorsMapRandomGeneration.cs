using UnityEngine;

public class ColorsMapRandomGeneration : IMapColorGenerator
{
    public int[,] GenerateMap(int x, int y)
    {
        var colorsMap = new int[x, y];

        for (int i = 0; i < x; i++)
        {
            for (int j = 0; j < y; j++)
            {
                var randomIndex = GetRandomColor();
                colorsMap[i, j] = randomIndex;
            }
        }

        return colorsMap;
    }

    public static int GetRandomColor()
    {
        var colorsAmount = ColorsMapDefinitions._colorDefinitions.Count;
        var randomIndex = Random.Range(0, colorsAmount - 1);
        return randomIndex;
    }
}

public interface IMapColorGenerator
{
    public int[,] GenerateMap(int x, int y);
}