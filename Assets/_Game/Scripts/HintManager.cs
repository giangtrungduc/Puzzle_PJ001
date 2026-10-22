using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class HintManager : MonoBehaviour
{
    public static HintManager Instance { get; private set; }

    public GameObject ghostPrefab;

    private GridManager grid;
    private GameObject currentGhost;
    private BlockEntity hintedBlock;
    private Vector2Int hintedTargetPos;
    private Tween pulseTween;

    void Awake()
    {
        Instance = this;
    }

    public void Init(GridManager gridManager)
    {
        this.grid = gridManager;
        ClearHint();
    }

    public void ShowHint()
    {
        ClearHint();

        List<GameObject> blockObjs = LevelSpawner.Instance.GetSpawnedBlocks();
        List<BlockEntity> entities = new List<BlockEntity>();
        foreach (var obj in blockObjs)
        {
            if (obj != null) entities.Add(obj.GetComponent<BlockEntity>());
        }

        Vector2Int exitPos = LevelSpawner.Instance.currentData.exitGridPos;
        SolverStep step = PuzzleSolver.Solve(entities, grid, exitPos);

        if (step == null || step.blockIndex < 0) return;

        hintedBlock = entities[step.blockIndex];
        hintedTargetPos = step.targetGridPos;

        pulseTween = hintedBlock.transform.DOPunchScale(Vector3.one * 0.15f, 0.6f, 1, 0.5f).SetLoops(-1);

        SpawnGhost(hintedBlock, hintedTargetPos);
    }

    private void SpawnGhost(BlockEntity block, Vector2Int targetPos)
    {
        if (ghostPrefab == null) return;

        currentGhost = Instantiate(ghostPrefab);

        float offset = (block.length - 1) * grid.cellSize / 2f;
        Vector3 worldPos = grid.GridToWorldPos(targetPos.x, targetPos.y);
        if (block.direction == BlockDirection.Horizontal) worldPos.x += offset;
        else worldPos.y += offset;

        currentGhost.transform.position = worldPos;

        SpriteRenderer ghostSr = currentGhost.GetComponent<SpriteRenderer>();
        if (ghostSr != null)
        {
            Color c = block.isTargetBlock ? Color.red : Color.yellow;
            c.a = 0.35f;
            ghostSr.color = c;

            if (block.direction == BlockDirection.Horizontal)
            {
                ghostSr.size = new Vector2(block.length * grid.cellSize - 0.1f, grid.cellSize - 0.1f);
            }
            else
            {
                ghostSr.size = new Vector2(grid.cellSize - 0.1f, block.length * grid.cellSize - 0.1f);
            }
        }
    }

    public void OnBlockMoved(BlockEntity block, Vector2Int newPos)
    {
        if (hintedBlock != null && block == hintedBlock)
        {
            if (newPos == hintedTargetPos)
            {
                ClearHint();
            }
        }
    }

    public void ClearHint()
    {
        if (pulseTween != null) pulseTween.Kill();
        if (hintedBlock != null) hintedBlock.transform.localScale = Vector3.one;

        if (currentGhost != null)
        {
            Destroy(currentGhost);
            currentGhost = null;
        }

        hintedBlock = null;
    }
}