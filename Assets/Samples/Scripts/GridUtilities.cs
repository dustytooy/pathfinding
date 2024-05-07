using UnityEngine;
using Dustytoy.Pathfinding.Grid;

namespace Dustytoy.Samples
{
    public static class GridUtilities
    {
        public static ICell PositionToCell(this IGrid grid, Vector2 position, float cellSize)
        {
            int x = (int)(position.x / cellSize);
            int y = (int)(position.y / cellSize);
            if (grid.IsValidPosition(x, y))
            {
                return grid.GetCell(x, y);
            }
            return null;
        }
    }
}
