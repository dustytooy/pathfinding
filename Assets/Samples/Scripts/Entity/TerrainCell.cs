using Dustytoy.Collections;

namespace Dustytoy.Samples.Grid2D.Entity
{
    internal interface ITerrainCell : IGrid2DItem
    {
        public TerrainProperties terrainProperties { get; set; }
        public ITerrainGrid grid { get; }
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
        public ITerrainGrid grid { get; private set; }

        public TerrainCell(int xCoordinate, int yCoordinate, TerrainProperties terrainProperties, ITerrainGrid grid)
        {
            this.xCoordinate = xCoordinate;
            this.yCoordinate = yCoordinate;
            this.terrainProperties = terrainProperties;
            this.grid = grid;
            grid.cells[grid.ToIndex(xCoordinate, yCoordinate)] = this;
        }
    }
}
