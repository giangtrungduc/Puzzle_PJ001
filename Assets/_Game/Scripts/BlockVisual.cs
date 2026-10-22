using UnityEngine;

public class BlockVisual : MonoBehaviour
{
    [SerializeField] private SpriteRenderer sr;
    [SerializeField] private BoxCollider2D col;
    [SerializeField] private Color targetColor = Color.red;
    [SerializeField] private Color normalColor = Color.yellow;

    public void SetupVisual(int length, BlockDirection dir, bool isTarget, float cellSize)
    {
        if(sr == null) return;

        if(isTarget) sr.color = targetColor;
        else sr.color = normalColor;

        if(dir == BlockDirection.Horizontal)
        {
            sr.size = new Vector2(length * cellSize - 0.1f, cellSize - 0.1f);
        }
        else
        {
            sr.size = new Vector2(cellSize - 0.1f, length * cellSize - 0.1f);
        }

        if(col == null) col = GetComponent<BoxCollider2D>();
        if(col != null) col.size = sr.size;
    }
}