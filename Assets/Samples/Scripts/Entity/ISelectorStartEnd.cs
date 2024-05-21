using System;

namespace Dustytoy.Samples.Grid2D.Entity
{
    internal interface ISelectorStartEnd
    {
        public ISelector selector { get; }
        public IPathfindingState pathfindingState { get; }

        public ITerrainCell start { get; }
        public ITerrainCell end { get; }
        public IObservable<ITerrainCell> onStartSelected { get; }
        public IObservable<ITerrainCell> onStartDeselected { get; }
        public IObservable<ITerrainCell> onEndSelected { get; }
        public IObservable<ITerrainCell> onEndDeselected { get; }
    }
}
