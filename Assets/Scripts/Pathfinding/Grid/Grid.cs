using UnityEngine;

namespace Pathfinding.Grid
{
    public class Grid
    {
        public int width { get; private set; }
        public int height { get; private set; }

        public Cell[] cells { get; private set; }
        public Cell final { get; set; }

        public Grid() { }

        public void Initialize(int width, int height, Cell[] cells, Vector2 goal)
        {
            this.width = width;
            this.height = height;

            var goalInt = Cell.PositionToInt(goal);
            if (cells == null || cells.Length != width * height || !IsValidPosition(goalInt.x, goalInt.y))
            {
                throw new System.ArgumentNullException("Grid constructor parameters");
            }

            this.cells = cells;
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
    }
}
