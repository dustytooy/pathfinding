namespace Dustytoy.Samples.Grid2D
{
    public struct PathfindingProperties
    {
        public enum State : int
        {
            None,
            OpenList,
            ClosedList,
            Start,
            End,
            Path
        }
        public enum Terrain : int
        {
            None,
            Obstacle,
        }
        public int gCost, hCost;
        public int fCost { get { return gCost + hCost; } }
        public State state;
        public Terrain terrain;
    }
}
