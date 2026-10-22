using UnityEngine;

[System.Serializable]
public class BlockSetupData
{
    public Vector2Int gridPos; // Tọa độ gốc
    public int length; // chiều dài
    public BlockDirection direction; // Hướng di chuyển
    public bool isTargetBlock; // Ô mục tiêu hay không
}
