using System.Collections.Generic;
using UnityEngine;

public class LevelSpawner : MonoBehaviour
{
    public static LevelSpawner Instance { get; private set; }

    public LevelData currentData;
    public GridManager grid;
    public GameObject blockPrefab;

    private List<GameObject> spawnedBlocks = new List<GameObject>();

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        LoadLevel(currentData);
    }

    public void LoadLevel(LevelData data)
    {
        currentData = data;
        ClearLevel();

        grid.Init(currentData.width, currentData.height);
        MoveHistoryManager.Instance.Init(grid);
        WinConditionChecker.Instance.Init(grid, currentData.exitGridPos);
        HintManager.Instance.Init(grid);

        foreach(BlockSetupData blockData in currentData.blocks)
        {
            GameObject block = Instantiate(blockPrefab);
            spawnedBlocks.Add(block);

            block.transform.position = grid.GridToWorldPos(blockData.gridPos.x, blockData.gridPos.y);
            float offset = (blockData.length - 1) * grid.cellSize / 2f;
            if(blockData.direction == BlockDirection.Horizontal)
            {
                block.transform.position += new Vector3(offset, 0, 0);
            }
            else
            {
                block.transform.position += new Vector3(0, offset, 0);
            }

            BlockEntity entity = block.GetComponent<BlockEntity>();
            entity.Init(blockData);
            block.GetComponent<BlockVisual>().SetupVisual(blockData.length, blockData.direction, blockData.isTargetBlock, grid.cellSize);
            block.GetComponent<BlockMovement>().Init(grid);

            foreach(Vector2Int cell in entity.GetOccupiedCells(entity.gridPos))
            {
                grid.SetCellOccupied(cell.x, cell.y, true);
            }
        }
    }

    public void ResetLevel()
    {
        LoadLevel(currentData);
    }

    private void ClearLevel()
    {
        for(int i = 0; i < spawnedBlocks.Count; i++)
        {
            if(spawnedBlocks[i] != null) Destroy(spawnedBlocks[i]);
        }
        spawnedBlocks.Clear();
    }

    public List<GameObject> GetSpawnedBlocks()
    {
        return spawnedBlocks;
    }
}