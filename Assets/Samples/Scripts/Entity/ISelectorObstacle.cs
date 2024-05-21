using System;

namespace Dustytoy.Samples.Grid2D.Entity
{
    internal interface ISelectorObstacle
    {
        public ISelector selector { get; }
        public IPathfindingState pathfindingState { get; }

        public IObservable<ITerrainCell> onObstacleSelected { get; }
        public IObservable<ITerrainCell> onObstacleDeselected { get; }
    }
}
