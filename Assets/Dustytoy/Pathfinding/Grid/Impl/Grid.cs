using Dustytoy.Collections;

namespace Dustytoy.Pathfinding.Grid
{
    public class Grid : Grid2D<ICell>, IGrid
    {
        public ICell[] cells { get { return array; } set { array = value; } }
        public Grid(int width, int height) : base(width, height)
        {
        }
    }
}
