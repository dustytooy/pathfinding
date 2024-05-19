using System;
using Dustytoy.DI;
using UniRx;

namespace Dustytoy.Samples.Grid2D.Entity
{
    internal interface IStartEndClicker
    {
        public ICellClicker cellClicker { get; }
        public IPathfindingState pathfindingState { get; }

        public ITerrainCell start { get; }
        public ITerrainCell end { get; }
        public IObservable<ITerrainCell> onStartSelected { get; }
        public IObservable<ITerrainCell> onStartDeselected { get; }
        public IObservable<ITerrainCell> onEndSelected { get; }
        public IObservable<ITerrainCell> onEndDeselected { get; }
    }
    internal class StartEndClicker : IStartEndClicker
    {
        [Inject]
        public ICellClicker cellClicker { get; private set; }
        [Inject]
        public IPathfindingState pathfindingState { get; private set; }

        public ITerrainCell start { get; private set; }
        public ITerrainCell end { get; private set; }
        public IObservable<ITerrainCell> onStartSelected { get; private set; }
        public IObservable<ITerrainCell> onStartDeselected { get; private set; }
        public IObservable<ITerrainCell> onEndSelected { get; private set; }
        public IObservable<ITerrainCell> onEndDeselected { get; private set; }

        private CompositeDisposable _disposables;
        public StartEndClicker()
        {
            _disposables = new CompositeDisposable();

            onStartSelected = cellClicker.onDifferentCellClicked.Where(_ => !_.IsObstacle() && pathfindingState.state == State.SelectStartPosition);
            onStartDeselected = cellClicker.onSameCellClicked.Where(_ => _ == start && pathfindingState.state == State.SelectStartPosition);
            onEndSelected = cellClicker.onDifferentCellClicked.Where(_ => !_.IsObstacle() && pathfindingState.state == State.SelectEndPosition);
            onEndDeselected = cellClicker.onSameCellClicked.Where(_ => _ == end && pathfindingState.state == State.SelectEndPosition);

            onStartSelected.Subscribe(_ => start = _).AddTo(_disposables);
            onStartDeselected.Subscribe(_ => start = null).AddTo(_disposables);
            onEndSelected.Subscribe(_ => end = _).AddTo(_disposables);
            onEndDeselected.Subscribe(_ => end = null).AddTo(_disposables);
        }
        ~StartEndClicker()
        {
            _disposables.Dispose();
        }
    }
}
