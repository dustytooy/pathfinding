using System;
using UniRx;

namespace Dustytoy.Samples.Grid2D.Entity.Impl
{
    internal class PathfindingState : IPathfindingState
    {
        public Entity.PathfindingState state { get => _state.Value; set => _state.Value = value; }
        public IObservable<Entity.PathfindingState> onStateChanged { get; private set; }
        public IObservable<Unit> onSelectObstaclesState { get; private set; }
        public IObservable<Unit> onSelectStartPositionState { get; private set; }
        public IObservable<Unit> onSelectEndPosition { get; private set; }
        public IObservable<Unit> onPathfindingState { get; private set; }
        public IObservable<Unit> onFinishedState { get; private set; }

        private ReactiveProperty<Entity.PathfindingState> _state;
        private CompositeDisposable _disposables;

        public PathfindingState()
        {
            _state = new ReactiveProperty<Entity.PathfindingState>(Entity.PathfindingState.SelectObstacles);
            onStateChanged = _state.Skip(1).DistinctUntilChanged();
            onSelectObstaclesState = onStateChanged.Where(x => x == Entity.PathfindingState.SelectObstacles).AsUnitObservable();
            onSelectStartPositionState = onStateChanged.Where(x => x == Entity.PathfindingState.SelectStartPosition).AsUnitObservable();
            onSelectEndPosition = onStateChanged.Where(x => x == Entity.PathfindingState.SelectEndPosition).AsUnitObservable();
            onPathfindingState = onStateChanged.Where(x => x == Entity.PathfindingState.Pathfinding).AsUnitObservable();
            onFinishedState = onStateChanged.Where(x => x == Entity.PathfindingState.Finished).AsUnitObservable();
        }
        ~PathfindingState()
        {
            _state.Dispose();
            _disposables.Dispose();
        }
    }
}
