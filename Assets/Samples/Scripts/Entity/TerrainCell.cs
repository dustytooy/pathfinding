using Dustytoy.Collections;

namespace Dustytoy.Samples.Grid2D.Entity
{
    internal interface ITerrainCell : IGrid2DItem
    {
        public TerrainProperties terrainProperties { get; set; }
    }
    internal static class TerrainCellUtilities
    {
        public static bool IsObstacle(this ITerrainCell self) => self.terrainProperties.type == TerrainProperties.Type.Obstacle;
    }
    internal class TerrainCell : ITerrainCell
    {
        public int xCoordinate { get; private set; }
        public int yCoordinate { get; private set; }
        public TerrainProperties terrainProperties { get; set; }

        public TerrainCell(int xCoordinate, int yCoordinate, TerrainProperties terrainProperties)
        {
            this.xCoordinate = xCoordinate;
            this.yCoordinate = yCoordinate;
            this.terrainProperties = terrainProperties;
        }
    }
}
