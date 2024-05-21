using Dustytoy.Collections;
using Dustytoy.DI;

namespace Dustytoy.Samples.Grid2D.Entity.Impl
{
    internal class TerrainGrid : Grid2D<ITerrainCell>, ITerrainGrid
    {
        public ITerrainCell[] cells => array;

        public TerrainGrid() : base(0,0)
        {
        }

        [Inject]
        public void Initialize(GridConfiguration config)
        {
            this.width = config.width;
            this.height = config.height;
            array = new ITerrainCell[width * height];

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    SetCell(x, y, new TerrainCell(x, y, default, this));
                }
            }
        }

        public void Reset()
        {
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    GetCell(x, y).terrainProperties = default;
                }
            }
        }
    }
}
