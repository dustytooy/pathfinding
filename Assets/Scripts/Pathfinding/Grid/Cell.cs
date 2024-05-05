using System.Collections.Generic;
using UnityEngine;

namespace Dustytoy.Pathfinding.Grid
{
    public class Cell : INode
    {
        public static readonly float size = 1f;
        public Vector2Int position { get; set; }

        public int gCost { get; set; }
        public int hCost { get; set; }
        public int fCost => gCost + hCost;
        public INode parent { get ; set; }
        public bool isObstacle { get ; set ; }

        private Grid _grid;
        private Vector2Int _goal;

        public Cell(Vector2Int position, bool isObstacle, Grid grid, Vector2Int goal)
        {
            this.position = position;
            this.isObstacle = isObstacle;
            gCost = 0;
            hCost = int.MaxValue;
            parent = null;

            _grid = grid;
            _goal = goal;
        }

        public bool Equals(INode other)
        {
            if(other == null) { return false; }
            if(other is Cell cell)
            {
                return position == cell.position;
            }
            return false;
        }

        public int CompareTo(INode other)
        {
            if (other == null) return 1;
            if(other is Cell)
            {
                if(fCost == other.fCost)
                {
                    return hCost.CompareTo(other.hCost);
                }
                else
                {
                    return fCost.CompareTo(other.fCost);
                }
            }
            return 1;
        }

        public INode[] GetNeighbors()
        {
            var neighbors = new List<Cell>();

            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    if (i == 0 && j == 0)
                        continue;
                    bool corner = (i == -1 && j == -1) || (i == -1 && j == 1) || (i == 1 && j == -1) || (i == 1 && j == 1);
                    int newX = position.x + i;
                    int newY = position.y + j;
                    var newCell = _grid[newX, newY];
                    if (corner)
                    {
                        var horizontal = _grid[position.x, newY];
                        var vertical = _grid[newX, position.y];
                        if((horizontal != null && horizontal.isObstacle) &&
                           (vertical != null && vertical.isObstacle))
                        {
                            continue;
                        }
                    }
                    if (newCell != null && !newCell.isObstacle)
                    {
                        neighbors.Add(newCell);
                    }
                }
            }

            return neighbors.ToArray();
        }

        public INode[] Traceback()
        {
            var trace = new List<INode>();
            var cell = this as INode;
            while (cell.parent != null)
            {
                trace.Add(cell);
                cell = cell.parent;
            }
            trace.Reverse();
            return trace.ToArray();
        }

        public int CalculateGCost(INode from)
        {
            var fromCell = from as Cell;
            return from.gCost + Mathf.FloorToInt((fromCell.position - position).magnitude * 10f);
        }

        public int CalculateHCost()
        {
            float dx = Mathf.Abs(position.x - _goal.x);
            float dy = Mathf.Abs(position.y - _goal.y);

            return Mathf.FloorToInt(size * (dx + dy + (Mathf.Sqrt(2f) - 2) * Mathf.Min(dx, dy)) * 10f);
        }

        public static Vector2Int PositionToInt(Vector2 position)
        {
            int x = (int)(position.x / size);
            int y = (int)(position.y / size);
            return new Vector2Int(x, y);
        }
    }
}
