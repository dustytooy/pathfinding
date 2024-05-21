using Dustytoy.Collections;

namespace Dustytoy.Samples.Grid2D.Entity
{
    internal interface ITerrainCell : IGrid2DItem
    {
        public TerrainProperties terrainProperties { get; set; }
        public ITerrainGrid grid { get; }
    }
}
