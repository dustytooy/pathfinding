using System;
using Dustytoy.DI;
using UniRx;

namespace Dustytoy.Samples.Grid2D.Entity.Impl
{
    internal class SelectorObstacle : ISelectorObstacle
    {
        [Inject]
        public ISelector selector { get; private set; }
        [Inject]
        public IPathfindingState pathfindingState { get; private set; }

        public IObservable<ITerrainCell> onObstacleSelected { get; private set; }
        public IObservable<ITerrainCell> onObstacleDeselected { get; private set; }

        private CompositeDisposable _disposables;
        public SelectorObstacle()
        {
            _disposables = new CompositeDisposable();

            onObstacleSelected = selector.onSelected.Where(_ => _.IsObstacle() && pathfindingState.state == Entity.PathfindingState.SelectObstacles);
            onObstacleDeselected = selector.onSelected.Where(_ => !_.IsObstacle() && pathfindingState.state == Entity.PathfindingState.SelectObstacles);

            onObstacleSelected.Subscribe(_ => _.terrainProperties = new TerrainProperties(TerrainProperties.Type.None)).AddTo(_disposables);
            onObstacleDeselected.Subscribe(_ => _.terrainProperties = new TerrainProperties(TerrainProperties.Type.Obstacle)).AddTo(_disposables);
        }
        ~SelectorObstacle()
        {
            _disposables.Dispose();
        }
    }
}
