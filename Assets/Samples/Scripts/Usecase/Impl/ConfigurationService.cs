using Dustytoy.Samples.Grid2D.Entity;
using Dustytoy.DI;
using System;
using UniRx;

namespace Dustytoy.Samples.Grid2D.Usecase.Impl
{
    internal class ConfigurationService : IConfigurationService, IDisposable
    {
        public IObservable<int> gridWidth => _gridConfiguration.Select(x => x.width);
        public IObservable<int> gridHeight => _gridConfiguration.Select(x => x.height);
        public IObservable<float> cellSize => _gridConfiguration.Select(x => x.cellSize);
        public IObservable<PathfindingMode> pathfindingMode => _pathfindingConfiguration.Select(x => x.pathfindingMode);

        [Inject]
        private ReactiveProperty<GridConfiguration> _gridConfiguration;
        [Inject]
        private ReactiveProperty<PathfindingConfiguration> _pathfindingConfiguration;

        public void SetGridDimensions(int width, int height)
        {
            _gridConfiguration.Value = new GridConfiguration()
            {
                width = width,
                height = height,
                cellSize = _gridConfiguration.Value.cellSize,
            };
        }

        public void SetCellSize(float cellSize)
        {
            _gridConfiguration.Value = new GridConfiguration()
            {
                width = _gridConfiguration.Value.width,
                height = _gridConfiguration.Value.height,
                cellSize = cellSize,
            };
        }

        public void SetPathfindingMode(PathfindingMode pathfindingMode)
        {
            _pathfindingConfiguration.Value = new PathfindingConfiguration()
            {
                pathfindingMode = pathfindingMode,
            };
        }

        public (int, int) GetGridDimensions()
        {
            return (_gridConfiguration.Value.width, _gridConfiguration.Value.height);
        }

        public float GetCellSize()
        {
            return _gridConfiguration.Value.cellSize;
        }

        public PathfindingMode GetPathfindingMode()
        {
            return _pathfindingConfiguration.Value.pathfindingMode;
        }

        public void Dispose()
        {
            _gridConfiguration.Dispose();
            _pathfindingConfiguration.Dispose();
        }
    }
}
