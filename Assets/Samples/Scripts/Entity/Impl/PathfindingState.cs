using System;
using UniRx;

namespace Dustytoy.Samples.Grid2D.Entity.Impl
{
    internal class PathfindingState : IPathfindingState
    {
        public State state { get => _state.Value; set => _state.Value = value; }
        public IObservable<State> onStateChanged { get; private set; }
        public IObservable<State> onSelectObstaclesState { get; private set; }
        public IObservable<State> onSelectStartPositionState { get; private set; }
        public IObservable<State> onSelectEndPosition { get; private set; }
        public IObservable<State> onPathfindingState { get; private set; }

        private ReactiveProperty<State> _state;
        private CompositeDisposable _disposables;

        public PathfindingState()
        {
            _state = new ReactiveProperty<State>(State.SelectObstacles);
            onStateChanged = _state.Skip(1).DistinctUntilChanged();
            onSelectObstaclesState = onStateChanged.Where(x => x == State.SelectObstacles);
            onSelectStartPositionState = onStateChanged.Where(x => x == State.SelectStartPosition);
            onSelectEndPosition = onStateChanged.Where(x => x == State.SelectEndPosition);
            onPathfindingState = onStateChanged.Where(x => x == State.Pathfinding);
        }
        ~PathfindingState()
        {
            _state.Dispose();
            _disposables.Dispose();
        }
    }
}
