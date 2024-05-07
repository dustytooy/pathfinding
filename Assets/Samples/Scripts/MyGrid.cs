using UnityEngine;
using UniRx;
using UniRx.Triggers;
using Dustytoy.DI;

namespace Dustytoy.Samples.Grid2D
{
    public class MyGrid : MonoBehaviour
    {
        public int width, height;
        [SerializeField]
        private RectTransform canvas;
        [SerializeField]
        private GameObject cellUIPrefab;
        [SerializeField]
        private bool displayGizmos;

        public MyCell[] cells { get; private set; }

        private GridPathfinder _pathfinder;

        [Inject]
        public void Initialize(GridPathfinder pathfinder)
        {
            var disposables = new CompositeDisposable();
            this.OnDestroyAsObservable().Subscribe(_ =>
            {
                _pathfinder = null;
            }).AddTo(disposables);

            _pathfinder = pathfinder;

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

                    // TODO: DI and Factory
                    var cell = cells[i] = go.GetComponent<MyCell>();
                    var ui = go.GetComponent<CellUI>();

                    Observable.NextFrame().Subscribe(_ =>
                    {
                        cell.Initialize();
                        cell.position = new Vector2(center.x, center.z);
                        cell.gCost.CombineLatest(cell.hCost, (x, y) => new Vector2Int(x, y)).Subscribe(ui.UpdateCost).AddTo(disposables);
                        cell.state.Subscribe(ui.UpdateColor).AddTo(disposables);
                        cell.terrain.Subscribe(ui.UpdateColor).AddTo(disposables);

                        ui.OnClick(() =>
                        {
                            _pathfinder.clickedCell.Value = cell;
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
                if (!keepTerrain)
                {
                    cell.terrain.Value = MyCell.Terrain.None;
                }
            }
        }

        private void OnDrawGizmos()
        {
            if (!displayGizmos) return;
            Gizmos.color = Color.yellow;
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Vector3 center = transform.position + new Vector3(x + 0.5f, 0, y + 0.5f) * MyCell.size;
                    Gizmos.DrawWireCube(center, Vector3.one * MyCell.size);
                }
            }
        }
    }
}
