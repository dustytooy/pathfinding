using Dustytoy.Collections;

namespace Dustytoy.Samples.Grid2D.Entity
{
    internal interface ITerrainGrid
    {
        public ITerrainCell[] cells { get; }
        public int width { get; }
        public int height { get; }
    }
    internal class TerrainGrid : Grid2D<ITerrainCell>
    {
        public ITerrainCell[] cells => array;
        public TerrainGrid(int width, int height) : base(width, height)
        {
        }
    }
}
