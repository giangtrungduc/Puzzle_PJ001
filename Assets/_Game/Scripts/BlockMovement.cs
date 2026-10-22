using UnityEngine;
using DG.Tweening;

public class BlockMovement : MonoBehaviour
{
    private BlockEntity entity;
    private GridManager grid;

    private Vector2 dragOffset;
    private Vector2Int startGridPos;
    private float minLimit;
    private float maxLimit;
    

    void Awake()
    {
        entity = GetComponent<BlockEntity>();
    }

    public void Init(GridManager gridManager)
    {
        this.grid = gridManager;
    }

    public void StartDrag(Vector2 pointerPos)
    {
        if(grid == null) return;
        transform.DOKill();

        foreach(Vector2Int cell in entity.GetOccupiedCells(entity.gridPos))
        {
            grid.SetCellOccupied(cell.x, cell.y, false);
        }

        dragOffset = (Vector2)transform.position - pointerPos;
        startGridPos = entity.gridPos;
        CalculateLimits();
    }

    public void ProcessDrag(Vector2 pointerPos)
    {
        Vector2 targetPos = pointerPos + dragOffset;

        if(entity.direction == BlockDirection.Horizontal)
        {
            float clampedX = Mathf.Clamp(targetPos.x, minLimit, maxLimit);
            transform.position = new Vector3(clampedX, transform.position.y, 0f);
        }
        else
        {
            float clampedY = Mathf.Clamp(targetPos.y, minLimit, maxLimit);
            transform.position = new Vector3(transform.position.x, clampedY, 0f);
        }
    }

    public void EndDrag()
    {
        float offset = (entity.length - 1) * grid.cellSize / 2f;
        Vector3 baseWorldPos = transform.position;
        if(entity.direction == BlockDirection.Horizontal) baseWorldPos.x -= offset;
        else baseWorldPos.y -= offset;

        Vector2Int newGridPos = grid.WorldToGridPos(baseWorldPos);
        entity.UpdateGridPos(newGridPos);

        foreach(Vector2Int cell in entity.GetOccupiedCells(entity.gridPos))
        {
            grid.SetCellOccupied(cell.x, cell.y, true);
        }

        Vector3 finalPos = grid.GridToWorldPos(newGridPos.x, newGridPos.y);
        if(entity.direction == BlockDirection.Horizontal) finalPos.x += offset;
        else finalPos.y += offset;

        transform.DOMove(finalPos, 0.15f).SetEase(Ease.OutQuad);

        HintManager.Instance.OnBlockMoved(entity, newGridPos);

        if(newGridPos != startGridPos)
        {
            MoveHistoryManager.Instance.RecordMove(entity, startGridPos, newGridPos);
        }

        WinConditionChecker.Instance.CheckWin(entity);
    }

    public void SnapToGridPos(Vector2Int targetGridPos)
    {
        float offset = (entity.length - 1) * grid.cellSize / 2f;
        Vector3 finalPos = grid.GridToWorldPos(targetGridPos.x, targetGridPos.y);
        if(entity.direction == BlockDirection.Horizontal) finalPos.x += offset;
        else finalPos.y += offset;

        transform.DOMove(finalPos, 0.15f).SetEase(Ease.OutQuad);
    }

    private void CalculateLimits()
    {
        int backSteps = 0;
        int forwardSteps = 0;

        if(entity.direction == BlockDirection.Horizontal)
        {
            while(grid.IsCellValidAndEmpty(entity.gridPos.x - 1 - backSteps, entity.gridPos.y))
            {
                backSteps++;
            }
            while(grid.IsCellValidAndEmpty(entity.gridPos.x + entity.length + forwardSteps, entity.gridPos.y))
            {
                forwardSteps++;
            }

            float offset = (entity.length - 1) * grid.cellSize / 2f;
            float currentCenter = grid.GridToWorldPos(entity.gridPos.x, entity.gridPos.y).x + offset;
            minLimit = currentCenter - backSteps * grid.cellSize;
            maxLimit = currentCenter + forwardSteps * grid.cellSize;
        }
        else
        {
            while(grid.IsCellValidAndEmpty(entity.gridPos.x, entity.gridPos.y - 1 - backSteps))
            {
                backSteps++;
            }
            while(grid.IsCellValidAndEmpty(entity.gridPos.x, entity.gridPos.y + entity.length + forwardSteps))
            {
                forwardSteps++;
            }

            float offset = (entity.length - 1) * grid.cellSize / 2f;
            float currentCenter = grid.GridToWorldPos(entity.gridPos.x, entity.gridPos.y).y + offset;
            minLimit = currentCenter - backSteps * grid.cellSize;
            maxLimit = currentCenter + forwardSteps * grid.cellSize;
        }
    }
}