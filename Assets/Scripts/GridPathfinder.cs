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


        //var cancellationToken = new CancellationTokenSource();
        //var waitForKeyDown = Observable.EveryUpdate()
        //    .Where(_ => Input.GetKeyDown(KeyCode.Space)).ToYieldInstruction();
        //var pathfindingStream = Observable.FromCoroutineValue<Request.PerStepData>(_ => RequestPathManager.Single(
        //    start,
        //    end,
        //    waitForKeyDown,
        //    cancellationToken.Token))
        //    .Subscribe(data =>
        //    {
        //        var status = data.status;
        //        Debug.Log($"Status: {status}");
        //        foreach(var node in data.addedToOpenList)
        //        {
        //            var open = node as Cell;
        //            int i = open.position.y * grid.width + open.position.x;
        //            var cell = grid.cells[i];
        //            cell.gCost.Value = node.gCost;
        //            cell.hCost.Value = node.hCost;
        //            cell.state.Value = MyCell.State.OpenList;
        //            Debug.Log($"Added {open.position} to open list");
        //        }
        //        {
        //            var closed = data.addedToClosedListOrGoal as Cell;
        //            int i = closed.position.y * grid.width + closed.position.x;
        //            var cell = grid.cells[i];
        //            if (start.Equals(closed))
        //            {
        //                cell.state.Value = MyCell.State.Start;
        //            }
        //            else
        //            {
        //                cell.state.Value = MyCell.State.ClosedList;
        //            }
        //            Debug.Log($"Added {closed.position} to {(data.status == Request.Status.InProgress ? "closed list" : "final")}");
        //        }
        //        if (status != Request.Status.InProgress)
        //        {
        //            path = null;
        //            _path = Array.ConvertAll(path, x =>
        //            {
        //                var c = x as Cell;
        //                int i = c.position.y * grid.width + c.position.x;
        //                var cell = grid.cells[i];
        //                if (!start.Equals(x) && !end.Equals(x))
        //                {
        //                    cell.state.Value = MyCell.State.Path;
        //                }

        //                var v = _pathGrid.CellToPosition(c);
        //                return grid.transform.position + new Vector3(v.x, 1f, v.y);
        //            });

        //            if (status == Request.Status.PathFound)
        //            {
        //                Debug.Log("Found path...");
        //            }
        //            else if (status == Request.Status.PathNotFound)
        //            {
        //                Debug.Log("Found no path...");
        //            }
        //        }
        //    }).AddTo(this);

        //var endKeyDown = Observable.EveryUpdate()
        //    .Where(_ => Input.GetKeyDown(KeyCode.Escape)).Subscribe(_=>
        //    {
        //        pathfindingStream.Dispose();
        //    });

        var status = RequestPathManager.RequestPathSimple(
            start,
            end,
            out path,
            grid.width * grid.height / 2,
            (x) =>
            {
                var c = x as Cell;
                int i = c.position.y * grid.width + c.position.x;
                var cell = grid.cells[i];
                cell.gCost.Value = x.gCost;
                cell.hCost.Value = x.hCost;
                cell.state.Value = MyCell.State.OpenList;
            },
            (x) =>
            {
                var c = x as Cell;
                int i = c.position.y * grid.width + c.position.x;
                var cell = grid.cells[i];
                if (start.Equals(x))
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

        if (status == Request.Status.PathFound)
        {
            Debug.Log("Found path...");
        }
        else if (status == Request.Status.PathNotFound)
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
        _path = null;
        _start = null;
        _end = null;
        Debug.Log("Cleaned references");
    }

    private void OnDestroy()
    {
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
