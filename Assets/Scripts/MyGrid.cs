using UnityEngine;
using R3;
using Pathfinding.Grid;

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

    private Pathfinding.Grid.Grid grid;
    private Vector2Int start, end;

    private Mode mode;
    private enum Mode
    {
        SelectStart,
        SelectEnd,
    }

    private void Start()
    {
        canvas.sizeDelta = new Vector2(width, height) * CellModel.size;
        canvas.position = transform.position + new Vector3(width * 0.5f, 0, height * 0.5f) * CellModel.size;
        Camera.main.orthographicSize = height * 0.5f;

        grid = new Pathfinding.Grid.Grid(width, height);

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Vector3 center = transform.position + new Vector3(x + 0.5f, 0, y + 0.5f) * CellModel.size;
                var go = Instantiate(costPrefab, center, canvas.rotation, canvas);
                go.name = $"{x}:{y}";
                int index = y * width + x;
                new CellViewModel(grid.cells[index], go.GetComponent<CellUIView>());
            }
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("Mouse clicked");
            if (mode == Mode.SelectStart)
            {
            }
            else if(mode == Mode.SelectEnd)
            {
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
                Vector3 center = transform.position + new Vector3(x + 0.5f, 0, y + 0.5f) * CellModel.size;
                Gizmos.DrawWireCube(center, Vector3.one * CellModel.size);
            }
        }

        Gizmos.color = Color.red;
        Vector3 s = transform.position + new Vector3(start.x + 0.5f, 0, start.y + 0.5f) * CellModel.size;
        Gizmos.DrawWireCube(s, Vector3.one * CellModel.size);
        Vector3 e = transform.position + new Vector3(end.x + 0.5f, 0, end.y + 0.5f) * CellModel.size;
        Gizmos.DrawWireCube(e, Vector3.one * CellModel.size);

    }
}
