using System;
using Dustytoy.DI;
using Dustytoy.Samples.Grid2D.Entity;
using UnityEngine;
using UniRx;

namespace Dustytoy.Samples.Grid2D.Usecase
{
    internal class PathfindingUsecase : IPathfindingUsecase
    {
        [Inject]
        public ISelector selector { get; private set; }
        [Inject]
        public IPathfindingState state { get; private set; }
        [Inject]
        public IPathfindingConfiguration pathfindingConfig { get; private set; }
        [Inject]
        public IPathfindingController controller { get; private set; }
        [Inject]
        public ITerrainGrid grid { get; private set; }

        public void Reset()
        {
            grid.Reset();
            state.state = PathfindingState.SelectObstacles;
        }

        public void Select(Vector2Int position)
        {
            selector.selected = grid.GetCell(position.x, position.y);
        }

        public void SelectObstacleMode()
        {
            state.state = PathfindingState.SelectObstacles;
        }

        public void SelectStartEndMode()
        {
            state.state = PathfindingState.SelectStartPosition;
        }

        public void Run()
        {
            state.state = PathfindingState.Pathfinding;
        }

        public void SetManualWaitSource(IObservable<Unit> source)
        {
            pathfindingConfig.manualWaitSource = source;
        }

        public void SetMode(PathfindingMode mode)
        {
            pathfindingConfig.pathfindingMode = mode;
        }

        public void SetOnCompleted(Action onComplete)
        {
            controller.onComplete = onComplete;
        }

        public void SetOnError(Action<Exception> onError)
        {
            controller.onError = onError;
        }

        public void SetOnNext(Action<StreamData> onNext)
        {
            controller.onNext = onNext;
        }
    }
}
