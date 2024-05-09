using System;
using System.Threading;
using Dustytoy.Pathfinding;
using Dustytoy.DI;
using UniRx;

namespace Dustytoy.Samples.Grid2D.Entity
{
    public enum State : int
    {
        SelectObstacles,
        SelectStartAndEndPositions,
        Pathfinding,
    }
    internal interface IPathfindingController
    {
        public IPathfindingConfiguration config { get; }
        public IPathfindingManager pathfindingManager { get; }
        public ITerrainCell startCell { get; set; }
        public ITerrainCell endCell { get; set; }
        public ITerrainCell[] path { get; }
        public IObservable<State> onStateChanged { get; }
        public IObservable<ITerrainCell> onCellClicked { get; }

        public void Cancel();
    }
    
    internal class PathfindingController : IPathfindingController
    {
        [Inject]
        public IPathfindingConfiguration config { get; private set; }
        [Inject]
        public IPathfindingManager pathfindingManager { get; private set; }
        public ITerrainCell startCell { get; set; }
        public ITerrainCell endCell { get; set; }
        public ITerrainCell[] path => _path;
        // TODO: add more class for separate of concerns
        public IObservable<State> onStateChanged { get; private set; }
        public IObservable<ITerrainCell> onCellClicked { get; private set; }

        public IObservable<StreamData> dataStream => _dataStream;

        private CancellationTokenSource _cancellationTokenSource;
        private ReactiveProperty<State> _state;
        private CompositeDisposable _disposables;
        private ITerrainCell[] _path;
        private IObservable<StreamData> _dataStream;

        [Inject]
        public void OnAwake()
        {
            _disposables = new CompositeDisposable();
            _state = new ReactiveProperty<State>(State.SelectObstacles).AddTo(_disposables);
            onStateChanged = _state.Skip(1).DistinctUntilChanged();
            // Cancel any current processing request
            onStateChanged.Where(_ => _ != State.Pathfinding).Subscribe(_ =>
            {
                Cancel();
            }).AddTo(_disposables);
            // Automatically start processing request
            onStateChanged.DistinctUntilChanged().Where(_ => _ == State.Pathfinding).Subscribe(_ =>
            {
                if (startCell == null || endCell == null)
                {
                    return;
                }
                Process();
            }).AddTo(_disposables);

            // To add interface for switching between state [Inject]

            Observable.WithLatestFrom(onCellClicked, onStateChanged, (x, y) => (x, y)).Where((tuple) => tuple.y == State.SelectObstacles).Subscribe((tuple) =>
            {
                var cell = tuple.x;
                var state = tuple.y;
                cell.terrainProperties = cell.IsObstacle() ? 
                new TerrainProperties(TerrainProperties.Type.None) : new TerrainProperties(TerrainProperties.Type.Obstacle);
            }).AddTo(_disposables);
            Observable.WithLatestFrom(onCellClicked, onStateChanged, (x, y) => (x, y)).Where((tuple) => tuple.y == State.SelectStartAndEndPositions).Subscribe((tuple) =>
            {
                var cell = tuple.x;
                var state = tuple.y;
                cell.terrainProperties = cell.IsObstacle() ?
                new TerrainProperties(TerrainProperties.Type.None) : new TerrainProperties(TerrainProperties.Type.Obstacle);
                if (cell.IsObstacle()) { return; }
                // Increment click count during select phase

                int click = 0;
                //click = _clickCount.Value = (_clickCount.Value + 1) % 2;
                // Selecting positions
                if (click == 1) // First click for start
                {
                    if (endCell != null)
                    {
                        // Reset state to none for cell

                        endCell = null;
                    }
                    if (startCell != null)
                    {
                        // Reset state to none for cell

                        startCell = null;
                    }
                    startCell = cell;
                    // Set state to Start for cell

                }
                else if (click == 0) // Second click for end
                {
                    endCell = cell;
                    // Set state to End for cell

                    // Set state to atomatic pathfinding, perhaps in a different class
                }
            }).AddTo(_disposables);
        }

        public void OnDestroy()
        {
            _disposables.Dispose();
            _cancellationTokenSource.Dispose();
            _path = null;
            _dataStream = null;
        }

        public void Process()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            GetPath(startCell, endCell, out _path, out _dataStream, _cancellationTokenSource.Token);
            // When using stream
            if(config.pathfindingMode != PathfindingMode.Instant)
            {
                Action<StreamData> onNext = null;
                Action<Exception> onError = null;
                Action onComplete = null;
                dataStream.Subscribe(onNext, onError, onComplete);
            }
        }

        //public void AlternateStateDuringStaging()
        //{
        //    _state.Value = _state.Value == State.SelectObstacles ? State.SelectStartAndEndPositions : State.SelectObstacles;
        //}
        //public void ResetState()
        //{
        //    _state.Value = State.SelectObstacles;
        //}

        public void Cancel()
        {
            if(_cancellationTokenSource != null && _cancellationTokenSource.Token.CanBeCanceled)
            {
                _cancellationTokenSource.Cancel();
            }
        }

        public void GetPath(ITerrainCell from, ITerrainCell to, out ITerrainCell[] result, out IObservable<StreamData> resultStream, CancellationToken cancellationToken = default)
        {
            result = null;
            resultStream = null;
            switch(config.pathfindingMode)
            {
                case PathfindingMode.Instant:
                    result = pathfindingManager.GetPath(from, to, cancellationToken);
                    break;
                case PathfindingMode.EveryFrame:
                    resultStream = pathfindingManager.GetPathStream(Observable.EveryUpdate().AsUnitObservable(), from, to, cancellationToken);
                    break;
                case PathfindingMode.EveryTimeStep:
                    resultStream = pathfindingManager.GetPathStream(Observable.Interval(TimeSpan.FromSeconds(config.timeStep)).AsUnitObservable(), from, to, cancellationToken);
                    break;
                case PathfindingMode.Manual:
                    resultStream = pathfindingManager.GetPathStream(config.waitSource, from, to, cancellationToken);
                    break;
            }
        }
    }
}
