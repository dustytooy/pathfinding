using System;

namespace Dustytoy.Samples.Grid2D.Entity
{
    public enum State : int
    {
        SelectObstacles,
        SelectStartPosition,
        SelectEndPosition,
        Pathfinding,
    }

    internal interface IPathfindingState
    {
        public State state { get; set; }
        public IObservable<State> onStateChanged { get; }
        public IObservable<State> onSelectObstaclesState { get; }
        public IObservable<State> onSelectStartPositionState { get; }
        public IObservable<State> onSelectEndPosition { get; }
        public IObservable<State> onPathfindingState { get; }
    }
}
