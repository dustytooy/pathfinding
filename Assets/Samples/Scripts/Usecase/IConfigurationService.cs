using Dustytoy.Samples.Grid2D.Entity;
using System;

namespace Dustytoy.Samples.Grid2D.Usecase
{
    internal interface IConfigurationService
    {
        public IObservable<int> gridWidth { get; }
        public IObservable<int> gridHeight { get; }
        public IObservable<float> cellSize { get; }
        public IObservable<PathfindingMode> pathfindingMode { get; }

        public void SetGridDimensions(int width, int height);
        public void SetCellSize(float cellSize);
        public void SetPathfindingMode(PathfindingMode pathfindingMode);
        public (int,int) GetGridDimensions();
        public float GetCellSize();
        public PathfindingMode GetPathfindingMode();
    }
}
