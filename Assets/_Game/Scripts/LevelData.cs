using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Level_", menuName = "BlockEscape/LevelData")]
public class LevelData : ScriptableObject
{
    public int width = 6;
    public int height = 6;

    public Vector2Int exitGridPos = new Vector2Int(5, 3);
    public List<BlockSetupData> blocks;
}
