using System.Collections.Generic;

namespace Dustytoy.Pathfinding.Grid
{
    public class Cell : ICell
    {
        public int xCoordinate { get; private set; }
        public int yCoordinate { get; private set; }
        public IGrid grid { get; private set; }

        public int gCost { get; set; }
        public int hCost { get; set; }
        public int fCost => gCost + hCost;
        public INode parent { get ; set; }
        public bool isObstacle { get ; set ; }

        private int _xGoalCoordinate, _yGoalCoordinate;

        public Cell(int x, int y, bool isObstacle, IGrid grid, int xGoalCoordinate, int yGoalCoordinate)
        {
            this.xCoordinate = x;
            this.yCoordinate = y;
            this.grid = grid;
            grid.cells[grid.ToIndex(xCoordinate, yCoordinate)] = this;

            this.isObstacle = isObstacle;
            gCost = 0;
            hCost = int.MaxValue;
            parent = null;

            _xGoalCoordinate = xGoalCoordinate;
            _yGoalCoordinate = yGoalCoordinate;
        }

        public bool Equals(INode other)
        {
            if(other == null) { return false; }
            if(other is Cell cell)
            {
                return xCoordinate == cell.xCoordinate && yCoordinate == cell.yCoordinate;
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
            var neighbors = new List<ICell>();

            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    if (i == 0 && j == 0)
                        continue;
                    bool corner = (i == -1 && j == -1) || (i == -1 && j == 1) || (i == 1 && j == -1) || (i == 1 && j == 1);
                    int newX = xCoordinate + i;
                    int newY = yCoordinate + j;
                    if (!grid.IsValidPosition(newX, newY))
                        continue;
                    var newCell = grid.GetCell(newX, newY);
                    if (corner)
                    {
                        var horizontal = grid.GetCell(xCoordinate, newY);
                        var vertical = grid.GetCell(newX, yCoordinate);
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
            var fromCell = from as ICell;
            return from.gCost + (int)(DistanceFrom(this, fromCell) * 10f);
        }

        public int CalculateHCost()
        {
            float dx = xCoordinate - _xGoalCoordinate;
            dx = dx < 0 ? -dx : dx;
            float dy = yCoordinate - _yGoalCoordinate;
            dy = dy < 0 ? -dy : dy;
            float min = dx < dy ? dx : dy;
            return (int)((dx + dy + 1.414f - 2f) * min * 10f);
        }

        private static float SqrDistanceFrom(ICell from, ICell to)
        {
            return (from.xCoordinate - to.xCoordinate) * (from.xCoordinate - to.xCoordinate) +
                (from.yCoordinate - to.yCoordinate) * (from.yCoordinate - to.yCoordinate);
        }
        private static float DistanceFrom(ICell from, ICell to)
        {
            return System.MathF.Sqrt(SqrDistanceFrom(from, to));
        }
    }
}
