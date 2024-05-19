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
        public IObservable<Unit> manualWaitSource { get; set; }
        public IObservable<Unit> waitSource { get; set; }
    }

    internal class PathfindingConfiguration : IPathfindingConfiguration
    {
        public PathfindingMode pathfindingMode { get => _pathfindingMode.Value; set => _pathfindingMode.Value = value; }
        public float timeStep { get => _timeStep.Value; set => _timeStep.Value = value; }
        public IObservable<Unit> manualWaitSource { get; set; }
        public IObservable<Unit> waitSource { get; set; }

        private ReactiveProperty<PathfindingMode> _pathfindingMode;
        private ReactiveProperty<float> _timeStep;
        private CompositeDisposable _disposables;

        public PathfindingConfiguration()
        {
            _pathfindingMode = new ReactiveProperty<PathfindingMode>(PathfindingMode.Instant);
            _timeStep = new ReactiveProperty<float> { Value = 0.1f };
            _disposables = new CompositeDisposable();
            _pathfindingMode.DistinctUntilChanged().Subscribe(_ =>
            {
                switch (_)
                {
                    case PathfindingMode.EveryFrame:
                        waitSource = Observable.EveryUpdate().AsUnitObservable();
                        break;
                    case PathfindingMode.EveryTimeStep:
                        waitSource = Observable.Interval(TimeSpan.FromSeconds(_timeStep.Value)).AsUnitObservable();
                        break;
                    case PathfindingMode.Manual:
                        waitSource = manualWaitSource;
                        break;
                    case PathfindingMode.Instant:
                        waitSource = null;
                        break;
                }
            }).AddTo(_disposables);
            _timeStep.DistinctUntilChanged().Where(_ => _pathfindingMode.Value == PathfindingMode.EveryTimeStep).Subscribe(_ =>
            {
                waitSource = Observable.Interval(TimeSpan.FromSeconds(_timeStep.Value)).AsUnitObservable();
            }).AddTo(_disposables);
        }

        ~PathfindingConfiguration()
        {
            _pathfindingMode.Dispose();
            _timeStep.Dispose();
            _disposables.Dispose();
        }
    }
}
