namespace Dustytoy.Samples.Grid2D.Entity
{
    internal interface ITerrainGrid
    {
        public ITerrainCell[] cells { get; }
        public int width { get; }
        public int height { get; }
        public void Initialize(GridConfiguration config);
        public void Reset();
        public int ToIndex(int x, int y);
        public bool IsValidPosition(int x, int y);
        public void SetCell(int x, int y, ITerrainCell cell);
        public ITerrainCell GetCell(int x, int y);
    }
}
