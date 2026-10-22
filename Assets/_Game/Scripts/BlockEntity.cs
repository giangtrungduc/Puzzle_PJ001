using System.Collections.Generic;
using UnityEngine;

public class BlockEntity : MonoBehaviour
{
    public Vector2Int gridPos { get; private set; }
    public int length { get; private set; }
    public BlockDirection direction { get; private set; }
    public bool isTargetBlock { get; private set; }

    public void Init(BlockSetupData data)
    {
        this.gridPos = data.gridPos;
        this.length = data.length;
        this.direction = data.direction;
        this.isTargetBlock = data.isTargetBlock;
    }

    public void UpdateGridPos(Vector2Int newPos)
    {
        this.gridPos = newPos;
    }

    public List<Vector2Int> GetOccupiedCells(Vector2Int basePos)
    {
        List<Vector2Int> ds = new List<Vector2Int>();
        for(int i = 0; i < length; i++)
        {
            Vector2Int pos;
            if(direction == BlockDirection.Horizontal)
            {
                pos = new Vector2Int(basePos.x + i, basePos.y);
            }
            else pos = new Vector2Int(basePos.x, basePos.y + i);

            ds.Add(pos);
        }
        return ds;
    }
}