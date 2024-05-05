using UnityEngine;
using UniRx;

public class MyGrid : MonoBehaviour
{
    public static MyGrid Instance { get { return _instance; } }
    private static MyGrid _instance;

    public int width, height;
    [SerializeField]
    private RectTransform canvas;
    [SerializeField]
    private GameObject cellUIPrefab;
    [SerializeField]
    private bool displayGizmos;

    public MyCell[] cells { get; private set; }

    private GridPathfinder _pathfinder;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    private void OnDestroy()
    {
        _instance = null;
    }

    public void Initialize()
    {
        _pathfinder = GridPathfinder.Instance;

        var disposables = new CompositeDisposable();

        canvas.sizeDelta = new Vector2(width, height) * MyCell.size;
        canvas.position = transform.position + new Vector3(width * 0.5f, 0, height * 0.5f) * MyCell.size;
        Camera.main.orthographicSize = height * 0.5f;
        Camera.main.transform.position = canvas.position + Vector3.up * 10;

        cells = new MyCell[width * height];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Vector3 center = transform.position + new Vector3(x + 0.5f, 0, y + 0.5f) * MyCell.size;
                var go = Instantiate(cellUIPrefab, center, canvas.rotation, canvas);
                go.name = $"{x}:{y}";
                int i = y * width + x;
                var cell = cells[i] = go.GetComponent<MyCell>();
                var ui = go.GetComponent<CellUI>();

                // Allow Start() in UI and cell components to initialize
                Observable.NextFrame().Subscribe(_ =>
                {
                    cell.position = new Vector2(center.x, center.z);
                    cell.gCost.CombineLatest(cell.hCost, (x, y) => new Vector2Int(x, y)).Subscribe(ui.UpdateCost).AddTo(disposables);
                    cell.state.Subscribe(ui.UpdateColor).AddTo(disposables);
                    cell.terrain.Subscribe(ui.UpdateColor).AddTo(disposables);

                    ui.OnClick(() =>
                    {
                        GridPathfinder.Instance.clickedCell.Value = cell;
                    }).AddTo(disposables);
                });
            }
        }


        // Begining of each phase (skip to avoid unnecessary clean up at start)
        _pathfinder.onPhaseChanged.Skip(1)
            .Where(x => x == GridPathfinder.Phase.SelectObstacles || x == GridPathfinder.Phase.SelectStartAndEndPositions)
            .Subscribe(_ =>
            {
                Clean(true);
            }).AddTo(disposables);

        disposables.AddTo(this);
    }

    public void Clean(bool keepTerrain)
    {
        foreach (var cell in cells)
        {
            cell.gCost.Value = -1;
            cell.hCost.Value = -1;
            cell.state.Value = MyCell.State.None;
            if(!keepTerrain)
            {
                cell.terrain.Value = MyCell.Terrain.None;
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (!displayGizmos) return;
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
