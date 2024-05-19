namespace Dustytoy.Collections
{
    public interface IGrid2DItem
    {
        public int yCoordinate { get; }
        public int xCoordinate { get; }
    }
    public class Grid2D<T> where T : IGrid2DItem
    {
        public int width { get; protected set; }
        public int height { get; protected set; }
        protected T[] array;

        public Grid2D(int width, int height)
        {
            this.width = width;
            this.height = height;
            array = new T[width * height];
        }

        public virtual bool IsValidPosition(int x, int y)
        {
            if (x < 0 || y < 0 || x >= width || y >= height)
            {
                return false;
            }
            return true;
        }

        public virtual T GetCell(int x, int y)
        {
            if (IsValidPosition(x, y))
                return array[ToIndex(x, y)];
            return default;
        }

        public virtual int ToIndex(int x, int y)
        {
            return y * width + x;
        }
    }
}
