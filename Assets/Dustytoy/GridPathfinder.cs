using System;
using System.Threading;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using Dustytoy.Pathfinding;
using Dustytoy.Pathfinding.Grid;
using Dustytoy.DI;

using Grid = Dustytoy.Pathfinding.Grid.Grid;

public class GridPathfinder : MonoBehaviour
{
    [SerializeField]
    private MyGrid grid;
    [SerializeField]
    private bool displayGizmos;

    private CancellationTokenSource _cancellationTokenSource;

    public enum Phase : int
    {
        SelectObstacles,
        SelectStartAndEndPositions,
        Pathfinding,
    }
    public enum Mode : int
    {
        TimeStep,
        ManualStep
    }

    public IObservable<Phase> onPhaseChanged { get; private set; }
    public ReactiveProperty<Phase> phase { get; set; }
    public ReactiveProperty<MyCell> clickedCell { get; set; }
    public ReactiveProperty<Mode> mode { get; set; }
    public ReactiveProperty<float> intervalInSeconds { get; set; }

    private MyCell _start;
    private MyCell _end;
    private ReactiveProperty<int> _clickCount;
    private Vector3[] _path = null;

    private IPathfindingRequestProvider _pathfindingService;

    [Inject]
    public void Initialize(IPathfindingRequestProvider pathfinding)
    {
        var disposables = new CompositeDisposable();
        this.OnDestroyAsObservable().Subscribe(_ =>
        {
            _pathfindingService = null;
            _path = null;
            _start = null;
            _end = null;
        }).AddTo(disposables);

        _pathfindingService = pathfinding;

        phase = new ReactiveProperty<Phase>(Phase.SelectStartAndEndPositions).AddTo(disposables);
        clickedCell = new ReactiveProperty<MyCell>().AddTo(disposables);
        mode = new ReactiveProperty<Mode>(Mode.TimeStep).AddTo(disposables);
        intervalInSeconds = new ReactiveProperty<float>(0.2f).AddTo(disposables);
        _clickCount = new ReactiveProperty<int>(0).AddTo(disposables);
        onPhaseChanged = phase.DistinctUntilChanged();

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
        Observable.EveryUpdate()
            .Where(_ => Input.GetKeyUp(KeyCode.Escape))
            .Subscribe(_ =>
            {
                if (phase.Value == Phase.Pathfinding)
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
        // Convert data
        var pathfindingGrid = new Grid(grid.width, grid.height);
        var goalPosition = CellUtilities.PositionToInt(_end.position, MyCell.size);

        pathfindingGrid.cells = Array.ConvertAll(grid.cells, x => 
        {
            var position = CellUtilities.PositionToInt(x.position, MyCell.size);
            return new Cell( 
                position.x, position.y,
                x.terrain.Value == MyCell.Terrain.Obstacle, 
                pathfindingGrid,
                goalPosition.x, goalPosition.y);
        });
        if(!pathfindingGrid.IsValidPosition(goalPosition.x, goalPosition.y))
        {
            throw new InvalidOperationException("Invalid position to pathfind!");
        }

        INode start = pathfindingGrid.PositionToCell(_start.position, MyCell.size);
        INode end = pathfindingGrid.PositionToCell(_end.position, MyCell.size);
        INode[] path;

        // Initialize request
        _cancellationTokenSource = new CancellationTokenSource();
        var (handle, request) = _pathfindingService.New(start, end, _cancellationTokenSource.Token);

        // Initialize step stream
        IObservable<Unit> stepStream;
        if(mode.Value == Mode.ManualStep)
        {
            stepStream = Observable.EveryUpdate().Where(_ => Input.GetKeyUp(KeyCode.Space))
            .TakeWhile(_ => !request.isDone || _cancellationTokenSource.IsCancellationRequested)
            .AsUnitObservable();
        }
        else if (mode.Value == Mode.TimeStep)
        {
            stepStream = Observable.Interval(TimeSpan.FromSeconds(intervalInSeconds.Value))
            .TakeWhile(_ => !request.isDone || _cancellationTokenSource.IsCancellationRequested)
            .AsUnitObservable();
        }
        else
        {
            stepStream = Observable.EveryUpdate()
                .TakeWhile(_ => !request.isDone || _cancellationTokenSource.IsCancellationRequested)
                .AsUnitObservable();
        }

        // Process request
        var disposable = request.ProcessStream(stepStream)
            .Finally(() =>
            {
                handle.Dispose();
                _cancellationTokenSource.Dispose();
                _cancellationTokenSource = null;
            })
            .Subscribe(
            data =>
            {
                var c = data.node as ICell;
                int i = c.yCoordinate * grid.width + c.xCoordinate;
                var cell = grid.cells[i];

                if (data.action == NodeAction.Start)
                {
                    cell.state.Value = MyCell.State.Start;
                }
                else if (data.action == NodeAction.End)
                {
                    cell.state.Value = MyCell.State.End;
                }
                else if (data.action == NodeAction.AddToOpenList && cell != _start) // comparison because starting cell is also added to open list per algo
                {
                    cell.gCost.Value = c.gCost;
                    cell.hCost.Value = c.hCost;
                    cell.state.Value = MyCell.State.OpenList;
                }
                else if (data.action == NodeAction.AddToClosedList && cell != _start) // comparison because starting cell is also added to open list per algo
                {
                    cell.state.Value = MyCell.State.ClosedList;
                }
            },
            (e) => 
            {
                if(e is OperationCanceledException)
                {
                    Debug.LogWarning(e);
                }
            },
            () =>
            {
                path = request.result;
                var status = request.pathfindingStatus;
                _path = Array.ConvertAll(path, x =>
                {
                    var c = x as Cell;
                    int i = c.yCoordinate * grid.width + c.xCoordinate;
                    var cell = grid.cells[i];

                    if (!start.Equals(x) && !end.Equals(x))
                    {

                        cell.state.Value = MyCell.State.Path;
                    }

                    var v = c.CellToPosition(MyCell.size);
                    return grid.transform.position + new Vector3(v.x, 1f, v.y);
                });
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
