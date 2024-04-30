using System.Collections.Generic;
using UnityEngine;

namespace Pathfinding.Grid
{
    public class Grid : IGraph<Cell>
    {
        public int width { get; private set; }
        public int height { get; private set; }

        public Cell[] cells { get; private set; }
        public Cell start { get ; set; }
        public Cell end { get ; set ; }
        public Cell final { get; set; }
        public IOpenList<Cell> openList { get ; set ; }
        public IClosedList<Cell> closedList { get; set; }

        public Grid(int width, int height, int[] inputs, Vector2 start, Vector2 end)
        {
            if (inputs == null || inputs.Length != width * height || start == null || end == null)
            {
                throw new System.ArgumentNullException("Grid constructor parameters");
            }

            this.width = width;
            this.height = height;

            cells = new Cell[inputs.Length];
            for(int y = 0; y < height; y++)
            {
                for(int x = 0; x < width; x++)
                {
                    int i = y * width + x;
                    cells[i] = new Cell(new Vector2Int(x, y), inputs[i] == 1);
                }
            }
            openList = new OpenList<Cell>();
            closedList = new ClosedList<Cell>();

            this.start = PositionToCell(start);
            this.end = PositionToCell(end);
        }

        public Cell this[int x, int y]
        {
            get
            {
                if(!IsValidPosition(x, y))
                    return null;
                return cells[y * width + x];
            }
        }
        public Cell this[int i]
        {
            get
            {
                if (i < 0 || i >= cells.Length)
                    return null;
                return cells[i];
            }
        }

        public bool IsValidPosition(int x, int y)
        {
            if(x < 0 || y < 0 || x >= width || y >= height)
            {
                return false;
            }
            return true;
        }

        public Cell[] TraceBack()
        {
            var trace = new List<Cell>();
            var cell = final;
            while(cell.parent != null)
            {
                trace.Add(cell);
                cell = cell.parent as Cell;
            }
            trace.Reverse();
            return trace.ToArray();
        }

        public bool IsStart(Cell cell)
        {
            return cell.Equals(start);
        }

        public bool IsEnd(Cell cell)
        {
            return cell.Equals(end);
        }

        public Cell PositionToCell(Vector2 position)
        {
            int x = (int)(position.x / Cell.size);
            int y = (int)(position.y / Cell.size);
            return this[x,y];
        }

        public Vector2 CellToPosition(Cell cell)
        {
            return new Vector2(cell.position.x + 0.5f, cell.position.y + 0.5f) * Cell.size;
        }

        public Cell[] GetNeighbors(Cell cell)
        {
            var neighbors = new List<Cell>();

            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    if (i == 0 && j == 0)
                        continue;
                    int newX = cell.position.x + i;
                    int newY = cell.position.y + j;
                    var newCell = this[newX, newY];
                    if (newCell != null && !newCell.isObstacle)
                    {
                        neighbors.Add(newCell);
                    }
                }
            }

            return neighbors.ToArray();
        }

        public int CalculateGCost(Cell current, Cell parent)
        {
            return parent.gCost + Mathf.FloorToInt((parent.position - current.position).magnitude * 10f);
        }

        public int CalculateHCost(Cell current)
        {
            float dx = Mathf.Abs(current.position.x - end.position.x);
            float dy = Mathf.Abs(current.position.y - end.position.y);

            return Mathf.FloorToInt(Cell.size * (dx + dy + (Mathf.Sqrt(2f) - 2) * Mathf.Min(dx, dy)) * 10f);
        }
    }
}
