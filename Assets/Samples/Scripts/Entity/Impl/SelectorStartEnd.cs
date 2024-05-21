using System;
using Dustytoy.DI;
using UniRx;

namespace Dustytoy.Samples.Grid2D.Entity.Impl
{
    internal class SelectorStartEnd : ISelectorStartEnd
    {
        [Inject]
        public ISelector selector { get; private set; }
        [Inject]
        public IPathfindingState pathfindingState { get; private set; }

        public ITerrainCell start { get; private set; }
        public ITerrainCell end { get; private set; }
        public IObservable<ITerrainCell> onStartSelected { get; private set; }
        public IObservable<ITerrainCell> onStartDeselected { get; private set; }
        public IObservable<ITerrainCell> onEndSelected { get; private set; }
        public IObservable<ITerrainCell> onEndDeselected { get; private set; }

        private CompositeDisposable _disposables;
        public SelectorStartEnd()
        {
            _disposables = new CompositeDisposable();

            onStartSelected = selector.onDifferentSelected.Where(_ => !_.IsObstacle() && pathfindingState.state == State.SelectStartPosition);
            onStartDeselected = selector.onSameSelected.Where(_ => _ == start && pathfindingState.state == State.SelectStartPosition);
            onEndSelected = selector.onDifferentSelected.Where(_ => !_.IsObstacle() && pathfindingState.state == State.SelectEndPosition);
            onEndDeselected = selector.onSameSelected.Where(_ => _ == end && pathfindingState.state == State.SelectEndPosition);

            onStartSelected.Subscribe(_ => start = _).AddTo(_disposables);
            onStartDeselected.Subscribe(_ => start = null).AddTo(_disposables);
            onEndSelected.Subscribe(_ => end = _).AddTo(_disposables);
            onEndDeselected.Subscribe(_ => end = null).AddTo(_disposables);
        }
        ~SelectorStartEnd()
        {
            _disposables.Dispose();
        }
    }
}
