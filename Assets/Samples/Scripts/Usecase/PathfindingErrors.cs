using System;

namespace Dustytoy.Samples.Grid2D.Usecase
{
    public static class PathfindingErrors
    {
        public static readonly Exception InvalidPositionException = new InvalidOperationException($"[Usecase] InvalidPositionException: is not a valid position on the grid!");
        public static readonly Exception StartEndNotInitializedException = new InvalidOperationException($"[Usecase] StartEndNotInitializedException: Either start or end position is not initialized!");
        public static readonly Exception GridNotInitializedException = new InvalidOperationException($"[Usecase] GridNotInitializedException: Grid is not initialized!");
        public static readonly Exception OperationCanceledException = new OperationCanceledException($"[Usecase] OperationCancelledException: Pathfinding request has been cancelled!");
    }
}
