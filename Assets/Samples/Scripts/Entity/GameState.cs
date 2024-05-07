namespace Dustytoy.Samples.Grid2D
{
    public enum State : int
    {
        SelectObstacles,
        SelectStartAndEndPositions,
        Pathfinding,
    }

    internal struct GameState
    {
        public State state;
    }
}
