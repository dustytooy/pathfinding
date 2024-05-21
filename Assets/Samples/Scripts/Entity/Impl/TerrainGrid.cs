using Dustytoy.Collections;

namespace Dustytoy.Samples.Grid2D.Entity.Impl
{
    internal class TerrainGrid : Grid2D<ITerrainCell>, ITerrainGrid
    {
        public ITerrainCell[] cells => array;
        public TerrainGrid(int width, int height) : base(width, height)
        {
        }
    }
}
