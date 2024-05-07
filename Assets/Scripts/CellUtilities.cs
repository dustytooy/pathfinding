using UnityEngine;
using Dustytoy.Pathfinding.Grid;

public static class CellUtilities
{
    public static Vector2 CellToPosition(this ICell cell, float cellSize)
    {
        return new Vector2(cell.xCoordinate + 0.5f, cell.yCoordinate + 0.5f) * cellSize;
    }

    public static Vector2Int PositionToInt(Vector2 position, float cellSize)
    {
        int x = (int)(position.x / cellSize);
        int y = (int)(position.y / cellSize);
        return new Vector2Int(x, y);
    }
}
