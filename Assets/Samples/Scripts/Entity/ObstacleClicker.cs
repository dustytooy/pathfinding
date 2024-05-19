using System;
using Dustytoy.DI;
using UniRx;

namespace Dustytoy.Samples.Grid2D.Entity
{
    internal interface IObstacleClicker
    {
        public ICellClicker cellClicker { get; }
        public IPathfindingState pathfindingState { get; }

        public IObservable<ITerrainCell> onObstacleSelect { get; }
        public IObservable<ITerrainCell> onObstacleDeselect { get; }
    }
    internal class ObstacleClicker : IObstacleClicker
    {
        [Inject]
        public ICellClicker cellClicker { get; private set; }
        [Inject]
        public IPathfindingState pathfindingState { get; private set; }

        public IObservable<ITerrainCell> onObstacleSelect { get; private set; }
        public IObservable<ITerrainCell> onObstacleDeselect { get; private set; }

        private CompositeDisposable _disposables;
        public ObstacleClicker()
        {
            _disposables = new CompositeDisposable();

            onObstacleSelect = cellClicker.onCellClicked.Where(_ => _.IsObstacle() && pathfindingState.state == State.SelectObstacles);
            onObstacleDeselect = cellClicker.onCellClicked.Where(_ => !_.IsObstacle() && pathfindingState.state == State.SelectObstacles);

            onObstacleSelect.Subscribe(_ => _.terrainProperties = new TerrainProperties(TerrainProperties.Type.None)).AddTo(_disposables);
            onObstacleDeselect.Subscribe(_ => _.terrainProperties = new TerrainProperties(TerrainProperties.Type.Obstacle)).AddTo(_disposables);
        }
        ~ObstacleClicker()
        {
            _disposables.Dispose();
        }
    }
}
