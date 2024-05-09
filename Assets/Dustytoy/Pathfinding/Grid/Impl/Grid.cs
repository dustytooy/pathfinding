namespace Dustytoy.Pathfinding.Grid
{
    public class Grid : IGrid
    {
        public int width { get; private set; }
        public int height { get; private set; }
        public ICell[] cells { get; set; }

        public Grid(int width, int height)
        {
            this.width = width;
            this.height = height;
        }

        public bool IsValidPosition(int x, int y)
        {
            if(x < 0 || y < 0 || x >= width || y >= height)
            {
                return false;
            }
            return true;
        }

        public ICell GetCell(int x, int y)
        {
            if(IsValidPosition(x, y))
                return cells[ToIndex(x, y)];
            return null;
        }

        public int ToIndex(int x, int y)
        {
            return y * width + x;
        }
    }
}
