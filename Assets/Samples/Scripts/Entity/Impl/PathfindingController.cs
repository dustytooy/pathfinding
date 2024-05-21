using System;
using System.Threading;
using Dustytoy.DI;
using Dustytoy.Pathfinding.Grid;
using UniRx;

namespace Dustytoy.Samples.Grid2D.Entity.Impl
{
    internal class PathfindingController : IPathfindingController
    {
        [Inject]
        public IPathfindingConfiguration pathfindingConfig { get; private set; }
        [Inject]
        public IPathfindingService pathfindingService { get; private set; }
        [Inject]
        public IPathfindingState pathfindingState { get; private set; }
        [Inject]
        public ISelectorObstacle obstacleClicker { get; private set; }
        [Inject]
        public ISelectorStartEnd startEndClicker { get; private set; }

        public ITerrainCell start => startEndClicker.start;
        public ITerrainCell end => startEndClicker.end;
        public ITerrainCell[] path => _path;

        public Action<StreamData> onNext { get; set; }
        public Action<Exception> onError { get; set; }
        public Action onComplete { get; set; }

        public IObservable<StreamData> dataStream => _dataStream;

        private CancellationTokenSource _cancellationTokenSource;
        private CompositeDisposable _disposables;
        private ITerrainCell[] _path;
        private IObservable<StreamData> _dataStream;

        public PathfindingController()
        {
            _disposables = new CompositeDisposable();
            // Cancellation when resetting to select mode
            pathfindingState.onSelectObstaclesState.Subscribe(_ =>
            {
                Cancel();
            }).AddTo(_disposables);
            pathfindingState.onSelectStartPositionState.Subscribe(_ =>
            {
                Cancel();
            }).AddTo(_disposables);

            // Running when state changed to running
            pathfindingState.onPathfindingState.Subscribe(_ =>
            {
                Run();
            }).AddTo(_disposables);

            // TODO: pathfinding state controlled in usecase
            // TODO: cell clicker controlled in usecase
            // TODO: automated process relevant to state transition
        }
        ~PathfindingController()
        {
            _disposables.Dispose();
            _cancellationTokenSource?.Dispose();
            _path = null;
            _dataStream = null;
        }

        public void Run()
        {
            _cancellationTokenSource = new CancellationTokenSource();

            if(pathfindingConfig.pathfindingMode != PathfindingMode.Instant)
            {
                _dataStream = GetPathStream(start, end, _cancellationTokenSource.Token);
                _dataStream.Subscribe(onNext, onError, onComplete);
            }
            else
            {
                _path = GetPath(start, end, _cancellationTokenSource.Token);
            }
        }

        public void Cancel()
        {
            if(_cancellationTokenSource != null && _cancellationTokenSource.Token.CanBeCanceled)
            {
                _cancellationTokenSource.Cancel();
                _cancellationTokenSource.Dispose();
            }
        }

        public ITerrainCell[] GetPath(ITerrainCell from, ITerrainCell to, CancellationToken cancellationToken = default)
        {
            int width = from.grid.width;
            int height = from.grid.height;
            var grid =  new Grid(width, height);
            var path = pathfindingService.GetPath(
                ConvertToNode(from, grid, to), 
                ConvertToNode(to, grid, to), 
                cancellationToken);
            ITerrainCell[] result = new ITerrainCell[path.Length];
            for(int i = 0; i < path.Length; i++)
            {
                result[i] = ConvertToTerrainCell(path[i] as ICell, from.grid);
            }
            return result;
        }

        public IObservable<StreamData> GetPathStream(ITerrainCell from, ITerrainCell to, CancellationToken cancellationToken = default)
        {
            int width = from.grid.width;
            int height = from.grid.height;
            var grid = new Grid(width, height);
            IObservable<Unit> waitSource = pathfindingConfig.waitSource;
            return pathfindingService.GetPathStream(
                waitSource,
                ConvertToNode(from, grid, to),
                ConvertToNode(to, grid, to),
                cancellationToken);
        }

        private ICell ConvertToNode(ITerrainCell cell, IGrid grid, ITerrainCell to)
        {
            return new Cell(cell.xCoordinate, cell.yCoordinate, cell.IsObstacle(), grid, to.xCoordinate, to.yCoordinate);
        }
        private ITerrainCell ConvertToTerrainCell(ICell node, ITerrainGrid grid)
        {
            return grid.GetCell(node.xCoordinate, node.yCoordinate);
        }
    }
}
