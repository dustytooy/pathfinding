using System;

namespace Dustytoy.Samples.Grid2D.Entity
{
    internal interface IPathfindingController
    {
        public IPathfindingConfiguration pathfindingConfig { get; }
        public IPathfindingService pathfindingService { get; }
        public IPathfindingState pathfindingState { get; }

        public ITerrainCell start { get; }
        public ITerrainCell end { get; }
        public ITerrainCell[] path { get; }
        public IObservable<StreamData> dataStream { get; }

        public Action<StreamData> onNext { get; set; }
        public Action<Exception> onError { get; set; }
        public Action onComplete { get; set; }

        public void Cancel();
        public void Run();
    }
}
