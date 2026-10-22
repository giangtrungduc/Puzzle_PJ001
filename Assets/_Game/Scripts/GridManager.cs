using UnityEngine;

public class GridManager : MonoBehaviour
{
    [Header("Grid Settings")]
    public float cellSize = 1f;
    public GameObject tilePrefab;

    private int width = 6;
    private int height = 6;
    private bool[,] occupiedCells;
    private Vector2 originPos;

    public void Init(int w, int h)
    {
        width = w;
        height = h;
        occupiedCells = new bool[width, height];

        float offsetX = -(width * cellSize) / 2f + (cellSize / 2f);
        float offsetY = -(height * cellSize) / 2f + (cellSize / 2f);
        originPos = new Vector2(offsetX, offsetY - 1);

        SpawnGridTiles();
    }

    private void SpawnGridTiles()
    {
        if(tilePrefab == null) return;

        for(int x = 0; x < width; x++)
        {
            for(int y = 0; y < height; y++)
            {
                Vector3 worldPos = GridToWorldPos(x, y);
                GameObject tile = Instantiate(tilePrefab, worldPos, Quaternion.identity, this.transform);
                tile.name = $"Tile_{x}_{y}";
            }
        }
    }

    public Vector3 GridToWorldPos(int x, int y)
    {
        return new Vector3(originPos.x + x * cellSize, originPos.y + y * cellSize, 0f);
    }

    public Vector2Int WorldToGridPos(Vector3 worldPos)
    {
        int x = Mathf.RoundToInt((worldPos.x - originPos.x) / cellSize);
        int y = Mathf.RoundToInt((worldPos.y - originPos.y) / cellSize);
        return new Vector2Int(x, y);
    }

    public bool IsCellValidAndEmpty(int x, int y)
    {
        if(x < 0 || x >= width || y < 0 || y >= height) return false;

        return !occupiedCells[x, y];
    }

    public void SetCellOccupied(int x, int y, bool isOccupied)
    {
        if(x >= 0 && x < width && y >= 0 && y < height)
        {
            occupiedCells[x, y] = isOccupied;
        }
    }
}