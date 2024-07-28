using UnityEngine;

namespace Data
{
    [CreateAssetMenu(menuName = "CreateCoreSettings", fileName = "CoreSettings")]
    public class CoreSettings : ScriptableObject
    {
         public int tilesSpace;
         public int boardSizeX;
         public int boardSizeY;
         public int minMatchesRequirement; //todo
         public int targetScore;
         public int targetTime;
    }
}