using Dustytoy.Samples.Grid2D.Entity;

namespace Dustytoy.Samples.Grid2D.Usecase
{
    internal interface IConfigurationService
    {
        public int gridWidth { get; set; }
        public int gridHeight { get; set; }
        public float cellSize { get; set; }
        public PathfindingMode pathfindingMode { get; set; }
    }
}
