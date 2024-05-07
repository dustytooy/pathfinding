using System;
using UnityEngine;

namespace Dustytoy.Samples.Grid2D
{
    public static class PathfindingErrors
    {
        public static Exception ThrowInvalidPositionException(Vector2 position, Exception e = null)
        {
            throw new InvalidOperationException($"[Usecase] InvalidPositionException: {position.ToString()} is not a valid position on the grid!", e);
        }
        public static Exception ThrowStartEndNotInitializedException(Exception e = null)
        {
            throw new InvalidOperationException($"[Usecase] StartEndNotInitializedException: Either start or end position is not initialized!", e);
        }
        public static Exception ThrowGridNotInitializedException(Exception e = null)
        {
            throw new InvalidOperationException($"[Usecase] GridNotInitializedException: Grid is not initialized!", e);
        }
        public static Exception ThrowOperationCancelledException(Exception e = null)
        {
            throw new OperationCanceledException($"[Usecase] OperationCancelledException: Pathfinding request has been cancelled!", e);
        }
    }
}
