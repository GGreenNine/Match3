using System.Collections.Generic;
using UnityEngine;

public static class ColorsMapDefinitions
{
    public static readonly Dictionary<int, Color> _colorDefinitions = new()
    {
        {-1, Color.clear},
        {0, Color.red},
        {1, Color.blue},
        {2, Color.green}
    };

    public static Color GetColor(int index)
    {
        _colorDefinitions.TryGetValue(index, out var color);
        return color;
    }
}