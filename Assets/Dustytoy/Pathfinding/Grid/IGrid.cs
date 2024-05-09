namespace Dustytoy.Pathfinding.Grid
{
    public interface IGrid
    {
        public int width { get; }
        public int height { get; }
        public ICell[] cells { get; set; }

        public bool IsValidPosition(int x, int y);
        public ICell GetCell(int x, int y);
        public int ToIndex(int x, int y);
    }
}
