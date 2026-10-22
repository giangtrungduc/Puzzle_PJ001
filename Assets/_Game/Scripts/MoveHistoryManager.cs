using System.Collections.Generic;
using UnityEngine;

public struct MoveRecord
{
    public BlockEntity block;
    public Vector2Int fromPos;
    public Vector2Int toPos;

    public MoveRecord(BlockEntity block, Vector2Int fromPos, Vector2Int toPos)
    {
        this.block = block;
        this.fromPos = fromPos;
        this.toPos = toPos;
    }
}

public class MoveHistoryManager : MonoBehaviour
{
    public static MoveHistoryManager Instance { get; private set; }

    private Stack<MoveRecord> history = new Stack<MoveRecord>();
    private GridManager grid;

    void Awake()
    {
        Instance = this;
    }

    public void Init(GridManager gridManager)
    {
        this.grid = gridManager;
        history.Clear();
    }

    public void RecordMove(BlockEntity block, Vector2Int fromPos, Vector2Int toPos)
    {
        history.Push(new MoveRecord(block, fromPos, toPos));
    }

    public bool Undo()
    {
        if(history.Count == 0) return false;

        MoveRecord lastMove = history.Pop();
        BlockEntity block = lastMove.block;

        foreach(Vector2Int cell in block.GetOccupiedCells(block.gridPos))
        {
            grid.SetCellOccupied(cell.x, cell.y, false);
        }

        block.UpdateGridPos(lastMove.fromPos);

        foreach(Vector2Int cell in block.GetOccupiedCells(block.gridPos))
        {
            grid.SetCellOccupied(cell.x, cell.y, true);
        }

        block.GetComponent<BlockMovement>().SnapToGridPos(lastMove.fromPos);
        return true;
    }
}