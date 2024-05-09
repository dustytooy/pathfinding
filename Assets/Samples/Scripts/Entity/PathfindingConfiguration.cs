using System;
using UniRx;

namespace Dustytoy.Samples.Grid2D.Entity
{
    public enum PathfindingMode : int
    {
        Instant,
        EveryFrame,
        EveryTimeStep,
        Manual,
    }

    internal interface IPathfindingConfiguration
    {
        public PathfindingMode pathfindingMode { get; set; }
        public float timeStep { get; set; }
        public IObservable<Unit> waitSource { get; set; }
    }

    internal class PathfindingConfiguration : IPathfindingConfiguration
    {
        public PathfindingMode pathfindingMode { get; set; }
        public float timeStep { get; set; }
        public IObservable<Unit> waitSource { get; set; }
    }
}
