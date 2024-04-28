using UnityEngine;
namespace Pathfinding.Grid
{
    public class Grid
    {
        public int width { get; private set; }
        public int height { get; private set; }
        public CellModel[] cells { get; private set; }

        public Grid(int width, int height)
        {
            this.width = width;
            this.height = height;

            cells = new CellModel[width * height];
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int index = y * width + x;
                    cells[index] = new CellModel(new Vector2Int(x, y));
                }
            }
        }

        public CellModel GetCell(int x, int y)
        {
            if(ValidatePosition(x, y))
            {
                return cells[y * width + x];
            }
            return null;
        }

        public bool ValidatePosition(int x, int y)
        {
            if(x < 0 || y < 0 || x >= width || y >= height)
            {
                return false;
            }
            return true;
        }
    }
}
