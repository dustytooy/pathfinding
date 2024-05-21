using UniRx;
using System;

namespace Dustytoy.Samples.Grid2D.Entity
{
    public enum PathfindingState : int
    {
        SelectObstacles,
        SelectStartPosition,
        SelectEndPosition,
        Pathfinding,
        Finished,
    }

    internal interface IPathfindingState
    {
        public PathfindingState state { get; set; }
        public IObservable<PathfindingState> onStateChanged { get; }
        public IObservable<Unit> onSelectObstaclesState { get; }
        public IObservable<Unit> onSelectStartPositionState { get; }
        public IObservable<Unit> onSelectEndPosition { get; }
        public IObservable<Unit> onPathfindingState { get; }
        public IObservable<Unit> onFinishedState { get; }
    }
}
