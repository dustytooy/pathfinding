using UnityEngine;
using R3;

public class MyGrid : MonoBehaviour
{
    [SerializeField]
    private int width, height;
    [SerializeField]
    private RectTransform canvas;
    [SerializeField]
    private GameObject costPrefab;
    [SerializeField]
    private bool display;

    private MyCell[] _grid;

    private void Start()
    {
        canvas.sizeDelta = new Vector2(width, height) * MyCell.size;
        canvas.position = transform.position + new Vector3(width * 0.5f, 0, height * 0.5f) * MyCell.size;
        Camera.main.orthographicSize = height * 0.5f;

        _grid = new MyCell[width * height];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Vector3 center = transform.position + new Vector3(x + 0.5f, 0, y + 0.5f) * MyCell.size;
                var go = Instantiate(costPrefab, center, canvas.rotation, canvas);
                go.name = $"{x}:{y}";
                int i = y * width + x;
                var cell = _grid[i] = go.GetComponent<MyCell>();
                var ui = go.GetComponent<CellUI>();
                cell.gCost.Subscribe(ui.UpdateGCost);
                cell.hCost.Subscribe(ui.UpdateHCost);
                cell.isObstacle.Subscribe(ui.UpdateCellColor);
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (!display) return;
        Gizmos.color = Color.yellow;
        for(int y = 0; y < height; y++)
        {
            for(int x = 0; x < width; x++)
            {
                Vector3 center = transform.position + new Vector3(x + 0.5f, 0, y + 0.5f) * MyCell.size;
                Gizmos.DrawWireCube(center, Vector3.one * MyCell.size);
            }
        }
    }
}
