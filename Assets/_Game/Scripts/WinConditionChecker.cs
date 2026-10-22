using System;
using UnityEngine;
using DG.Tweening;

public class WinConditionChecker : MonoBehaviour
{
    public static WinConditionChecker Instance { get; private set; }
    public event Action OnLevelCompleted;

    private Vector2Int exitPos;
    private GridManager grid;
    private bool isWon = false;

    void Awake()
    {
        Instance = this;
    }

    public void Init(GridManager gridManager, Vector2Int exitGridPos)
    {
        this.grid = gridManager;
        this.exitPos = exitGridPos;
        this.isWon = false;
    }

    public void CheckWin(BlockEntity block)
    {
        if(isWon || !block.isTargetBlock) return;

        int rightEndCellX = block.gridPos.x + block.length - 1;
        if(rightEndCellX == exitPos.x && block.gridPos.y == exitPos.y)
        {
            isWon = true;
            OnLevelCompleted?.Invoke();

            float escapeTargetX = transform.position.x + 5f;
            block.transform.DOMoveX(block.transform.position.x + 4f, 0.5f).SetEase(Ease.InQuad);
        }
    }
}