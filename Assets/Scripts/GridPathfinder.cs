using System;
using UnityEngine;
using UniRx;
using Dustytoy.Pathfinding;
using Dustytoy.Pathfinding.Grid;
using Grid = Dustytoy.Pathfinding.Grid.Grid;

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

    private Grid _pathGrid;

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
        PathfindingManager.InitializePool();
        _pathGrid = new Grid();

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
                    }
                    else if(click == 0) // Second click for end
                    {
                        _end = cell;
                        _end.state.Value = MyCell.State.End;
                    }
                }
            });
    }

    public void Pathfinding()
    {
        Debug.Log("Finding path...");

        Cell[] cells = Array.ConvertAll(grid.cells, x => 
        {
            return new Cell(
                Cell.PositionToInt(x.position), 
                x.terrain.Value == MyCell.Terrain.Obstacle, 
                _pathGrid,
                Cell.PositionToInt(_end.position));
        });
        _pathGrid.Initialize(grid.width, grid.height, cells, _end.position);


        INode start = _pathGrid.PositionToCell(_start.position);
        INode end = _pathGrid.PositionToCell(_end.position);
        INode[] path;

        var (handle, request) = PathfindingManager.Request(start, end);

        Action<Request.StreamData> processStreamData = (x) =>
        {
            var c = x.node as Cell;
            int i = c.position.y * grid.width + c.position.x;
            var cell = grid.cells[i];

            if (x.action == Request.NodeAction.AddToOpenList)
            {
                cell.gCost.Value = c.gCost;
                cell.hCost.Value = c.hCost;
                cell.state.Value = MyCell.State.OpenList;
                Debug.Log($"Added {(x.node as Cell).position.ToString()} to Open List");
            }
            else if (x.action == Request.NodeAction.AddToClosedList)
            {
                cell.state.Value = MyCell.State.ClosedList;
                Debug.Log($"Added {(x.node as Cell).position.ToString()} to Closed List");
            }
            else if (x.action == Request.NodeAction.Start)
            {
                cell.state.Value = MyCell.State.Start;
                Debug.Log($"Start from {(x.node as Cell).position.ToString()}");
            }
            else if (x.action == Request.NodeAction.End)
            {
                cell.state.Value = MyCell.State.End;
                Debug.Log($"End at {(x.node as Cell).position.ToString()}");
            }
        };

        Action writePath = () =>
        {
            path = request.result;
            var status = request.pathfindingStatus;
            _path = Array.ConvertAll(path, x =>
            {
                var c = x as Cell;
                int i = c.position.y * grid.width + c.position.x;
                var cell = grid.cells[i];

                if (!start.Equals(x) && !end.Equals(x))
                {

                    cell.state.Value = MyCell.State.Path;
                }

                var v = _pathGrid.CellToPosition(c);
                return grid.transform.position + new Vector3(v.x, 1f, v.y);
            });

            if (status == Request.PathfindingStatus.PathFound)
            {
                Debug.Log("Found path...");
            }
            else if (status == Request.PathfindingStatus.PathNotFound)
            {
                Debug.Log("Found no path...");
            }
        };

        var interval = Observable.Interval(TimeSpan.FromSeconds(0.5))
            .Where(_ => !request.isDone)
            .AsUnitObservable();
        var disposable = request.ProcessStreamWaitable(
            interval,
            data =>
            {
                processStreamData(data);
            },
            (e) =>
            {
                Debug.LogError(e);
                handle.Release();
            },
            () =>
            {
                writePath();
                handle.Release();
            });
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
        return phase.Value;
    }

    private void Clean()
    {
        _path = null;
        _start = null;
        _end = null;
    }

    private void OnDestroy()
    {
        PathfindingManager.CleanPool();
        _pathGrid = null;
        _path = null;
        _start = null;
        _end = null;
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
