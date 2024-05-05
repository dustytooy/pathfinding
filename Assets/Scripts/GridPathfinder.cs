using System;
using System.Threading;
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
    private CancellationTokenSource _cancellationTokenSource;

    public enum Phase : int
    {
        SelectObstacles,
        SelectStartAndEndPositions,
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

    private void OnDestroy()
    {
        PathfindingManager.CleanPool();
        _pathGrid = null;
        _path = null;
        _start = null;
        _end = null;
        _instance = null;
    }

    public void Initialize()
    {
        PathfindingManager.InitializePool();
        _pathGrid = new Grid();

        phase = new ReactiveProperty<Phase>(Phase.SelectStartAndEndPositions).AddTo(this);
        clickedCell = new ReactiveProperty<MyCell>().AddTo(this);
        _clickCount = new ReactiveProperty<int>(0).AddTo(this);
        onPhaseChanged = phase.DistinctUntilChanged();

        var disposables = new CompositeDisposable();

        // TODO: Refactor state/phase management
        onPhaseChanged.Skip(1).Subscribe(x =>
        {
            switch (x)
            {
                case Phase.SelectObstacles: case Phase.SelectStartAndEndPositions:
                    Cancel();
                    break;
                case Phase.Pathfinding:
                    if (_start == null || _end == null)
                    {
                        Debug.LogWarning("Either start or end position is missing!");
                        return;
                    }
                    Pathfinding();
                    break;
            }
        }).AddTo(disposables);

        // Switch phases with keyboard
        Observable.EveryUpdate()
            .Where(_ => Input.GetKeyDown(KeyCode.LeftShift))
            .Subscribe(_ =>
            {
                if(phase.Value == Phase.SelectStartAndEndPositions)
                {
                    phase.Value = Phase.SelectObstacles;
                }
            }).AddTo(disposables);
        Observable.EveryUpdate()
            .Where(_ => Input.GetKeyUp(KeyCode.LeftShift))
            .Subscribe(_ =>
            {
                if (phase.Value == Phase.SelectObstacles)
                {
                    phase.Value = Phase.SelectStartAndEndPositions;
                }
            }).AddTo(disposables);

        // Staging obstacles with clicks
        var clickedCellStream = clickedCell.Skip(1);
        Observable.WithLatestFrom(clickedCellStream, onPhaseChanged,(x, y) => (x, y)).Where(data => data.y == Phase.SelectObstacles)
            .Subscribe(_ =>
            {
                var cell = _.x;
                cell.terrain.Value = cell.terrain.Value == MyCell.Terrain.Obstacle ? MyCell.Terrain.None : MyCell.Terrain.Obstacle;
                Debug.Log($"Modified terrain ({cell.position}: {cell.terrain.Value})");
                return;
            }).AddTo(disposables);
        // Staging start and end position
        Observable.WithLatestFrom(clickedCellStream, onPhaseChanged, (x, y) => (x, y)).Where(data => data.y == Phase.SelectStartAndEndPositions)
            .Subscribe(_ =>
            {
                var cell = _.x;
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
                    if (_start != null)
                    {
                        _start.state.Value = MyCell.State.None;
                        _start = null;
                    }
                    _start = cell;
                    _start.state.Value = MyCell.State.Start;
                }
                else if (click == 0) // Second click for end
                {
                    _end = cell;
                    _end.state.Value = MyCell.State.End;
                    phase.Value = Phase.Pathfinding;
                }
            }).AddTo(disposables);

        disposables.AddTo(this);
    }

    public void Pathfinding()
    {
        Debug.Log("Started request!");

        // Initialize data
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

        // Initialize request
        _cancellationTokenSource = new CancellationTokenSource();
        var (handle, request) = PathfindingManager.Request(start, end, _cancellationTokenSource.Token);

        // Process request
        var interval = Observable.Interval(TimeSpan.FromSeconds(0.1))
            .TakeWhile(_ => !request.isDone || _cancellationTokenSource.IsCancellationRequested)
            .AsUnitObservable();

        var disposable = request.ProcessStream(interval)
            .Finally(() =>
            {
                handle.Release();
                _cancellationTokenSource.Dispose();
                _cancellationTokenSource = null;
                Debug.Log("Ended request (released)!");
            })
            .Subscribe(
            data =>
            {
                var c = data.node as Cell;
                int i = c.position.y * grid.width + c.position.x;
                var cell = grid.cells[i];


                if (data.action == Request.NodeAction.Start)
                {
                    cell.state.Value = MyCell.State.Start;
                    Debug.Log($"Start from {(data.node as Cell).position.ToString()}");
                }
                else if (data.action == Request.NodeAction.End)
                {
                    cell.state.Value = MyCell.State.End;
                    Debug.Log($"End at {(data.node as Cell).position.ToString()}");
                }
                else if (data.action == Request.NodeAction.AddToOpenList && cell != _start) // comparison because starting cell is also added to open list per algo
                {
                    cell.gCost.Value = c.gCost;
                    cell.hCost.Value = c.hCost;
                    cell.state.Value = MyCell.State.OpenList;
                    Debug.Log($"Added {(data.node as Cell).position.ToString()} to Open List");
                }
                else if (data.action == Request.NodeAction.AddToClosedList && cell != _start) // comparison because starting cell is also added to open list per algo
                {
                    cell.state.Value = MyCell.State.ClosedList;
                    Debug.Log($"Added {(data.node as Cell).position.ToString()} to Closed List");
                }
            },
            (e) =>
            {
                if(e is  OperationCanceledException)
                {
                    Debug.Log("Cancelled request!");
                }
            },
            () =>
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
            });
    }

    public void Cancel()
    {
        if (_cancellationTokenSource != null && _cancellationTokenSource.Token.CanBeCanceled)
        {
            _cancellationTokenSource.Cancel();
        }
        Clean();
    }

    private void Clean()
    {
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
