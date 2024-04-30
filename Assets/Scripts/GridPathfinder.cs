using System;
using UnityEngine;
using Pathfinding;
using UniRx;

public class GridPathfinder : MonoBehaviour
{
    [SerializeField]
    private MyGrid grid;
    [SerializeField]
    private bool displayGizmos;

    public static GridPathfinder Instance { get { return _instance; } }
    private static GridPathfinder _instance;

    public enum Phase : int
    {
        Staging,
        Select,
        Pathfinding,
    }

    public IObservable<Phase> onPhaseChanged { get; private set; }
    public ReactiveProperty<Phase> phase { get; set; }
    public ReactiveProperty<MyCell> clickedCell { get; set; }

    private MyCell _start;
    private MyCell _end;
    private ReactiveProperty<int> _clickCount;
    private Vector3[] _path = null;

    private Pathfinding.Grid.Grid _pathGrid;

    private void Awake()
    {
        if(_instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    public void Initialize()
    {
        phase = new ReactiveProperty<Phase>(Phase.Staging);
        clickedCell = new ReactiveProperty<MyCell>();
        _clickCount = new ReactiveProperty<int>(0);
        onPhaseChanged = phase.DistinctUntilChanged();


        // Begining of each phase (skip to avoid unnecessary clean up at start)
        onPhaseChanged.Skip(1).Subscribe(_ =>
        {
            switch (_)
            {
                case Phase.Staging:
                    Clean();
                    break;
                case Phase.Select:
                    Clean();
                    break;
                case Phase.Pathfinding:
                    Pathfinding();
                    break;
            }
        });

        // Staging obstacles
        var clickedCellStream = clickedCell.Skip(1);
        Observable.WithLatestFrom(
            clickedCellStream,
            onPhaseChanged,
            (x, y) => (x,y)).Subscribe(_ =>
            {
                var cell = _.x;
                var phase = _.y;
                if (phase == Phase.Staging)
                {
                    cell.terrain.Value = cell.terrain.Value == MyCell.Terrain.Obstacle ? MyCell.Terrain.None : MyCell.Terrain.Obstacle;
                    Debug.Log($"Modified terrain ({cell.position}: {cell.terrain.Value})");
                    return;
                }
                else if(phase == Phase.Select)
                {
                    if (cell.terrain.Value == MyCell.Terrain.Obstacle) { return; }
                    // Increment click count during select phase
                    int click = _clickCount.Value = (_clickCount.Value + 1) % 2;
                    // Selecting positions
                    if (click == 1) // First click for start
                    {
                        if (_end != null)
                        {
                            _end.state.Value = MyCell.State.None;
                            _end = null;
                        }
                        _start = cell;
                        _start.state.Value = MyCell.State.Start;
                        Debug.Log($"Modified _start ({_start})");
                    }
                    else if(click == 0) // Second click for end
                    {
                        _end = cell;
                        _end.state.Value = MyCell.State.End;
                        Debug.Log($"Modified _end ({_end})");
                    }
                }
            });
    }

    public void Pathfinding()
    {
        Debug.Log("Finding path...");
        int[] obstacles = Array.ConvertAll(grid.cells, x => { return x.terrain.Value == MyCell.Terrain.Obstacle ? 1 : 0; });

        _pathGrid = new Pathfinding.Grid.Grid(grid.width, grid.height, obstacles, _start.position, _end.position);

        var (status, path) = RequestPathManager.ResolvePath(_pathGrid,
            (Pathfinding.Grid.Cell x) =>
            {
                int i = x.position.y * grid.width + x.position.x;
                var cell = grid.cells[i];
                cell.gCost.Value = x.gCost;
                cell.hCost.Value = x.hCost;
                cell.state.Value = MyCell.State.OpenList;
            },
            (Pathfinding.Grid.Cell x) =>
            {
                int i = x.position.y * grid.width + x.position.x;
                var cell = grid.cells[i];
                if (_pathGrid.IsStart(x))
                {
                    cell.state.Value = MyCell.State.Start;
                }
                else
                {
                    cell.state.Value = MyCell.State.ClosedList;
                }
            });

        _path = Array.ConvertAll(path, x =>
        {
            int i = x.position.y * grid.width + x.position.x;
            var cell = grid.cells[i];
            if (!_pathGrid.IsStart(x) && !_pathGrid.IsEnd(x))
            {
                cell.state.Value = MyCell.State.Path;
            }

            var v = _pathGrid.CellToPosition(x);
            return grid.transform.position + new Vector3(v.x, 1f, v.y);
        });

        if (status == RequestPathManager.Status.PathFound)
        {
            Debug.Log("Found path...");
        }
        else if (status == RequestPathManager.Status.PathNotFound)
        {
            Debug.Log("Found no path...");
        }
    }

    public Phase NextPhase()
    {
        switch (phase.Value)
        {
            case Phase.Staging:
                phase.Value = Phase.Select;
                break;
            // Auto rotate between Select and Pathfinding
            case Phase.Select:
                if(_start == null || _end == null)
                {
                    Debug.LogWarning("Either missing start or end cell!");
                    return Phase.Select;
                }
                phase.Value = Phase.Pathfinding;
                break;
            case Phase.Pathfinding:
                phase.Value = Phase.Select;
                break;
        }
        Debug.Log($"Phase ({phase.Value})");
        return phase.Value;
    }

    private void Clean()
    {
        _pathGrid = null;
        _path = null;
        _start = null;
        _end = null;
        Debug.Log("Cleaned references");
    }

    private void OnDrawGizmos()
    {
        if (!displayGizmos) return;
        if(_path == null || _path.Length == 0) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(_path[0], Vector3.one * MyCell.size * 0.5f);

        Gizmos.color = Color.green;
        for (int i = 1; i < _path.Length; i++)
        {
            Gizmos.DrawWireCube(_path[i], Vector3.one * MyCell.size * 0.5f);
        }
    }
}
