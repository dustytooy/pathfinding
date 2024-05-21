using System;
using UnityEngine;
using Dustytoy.Samples.Grid2D.Entity;
using UniRx;

namespace Dustytoy.Samples.Grid2D.Usecase
{
    internal interface IPathfindingUsecase
    {
        public void Select(Vector2Int position);
        public void Reset();
        public void SelectObstacleMode();
        public void SelectStartEndMode();
        public void SetManualWaitSource(IObservable<Unit> source);
        public void SetMode(PathfindingMode mode);
        public void SetOnCompleted(Action onComplete);
        public void SetOnError(Action<Exception> onError);
        public void SetOnNext(Action<StreamData> onNext);
    }
}
