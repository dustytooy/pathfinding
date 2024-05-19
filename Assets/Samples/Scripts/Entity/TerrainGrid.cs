using Dustytoy.Collections;

namespace Dustytoy.Samples.Grid2D.Entity
{
    internal interface ITerrainGrid
    {
        public ITerrainCell[] cells { get; }
        public int width { get; }
        public int height { get; }

        public int ToIndex(int x, int y);
        public bool IsValidPosition(int x, int y);
        public ITerrainCell GetCell(int x, int y);
    }
    internal class TerrainGrid : Grid2D<ITerrainCell>, ITerrainGrid
    {
        public ITerrainCell[] cells => array;
        public TerrainGrid(int width, int height) : base(width, height)
        {
        }
    }
}
