namespace Dustytoy.Samples.Grid2D.Entity
{
    public enum PathfindingMode : int
    {
        Instant,
        EveryFrame,
        EveryTimeStep,
        Manual,
    }

    internal struct PathfindingConfiguration
    {
        public PathfindingMode pathfindingMode;
    }
}
