namespace Pathfinding
{
    public struct Cost
    {
        public enum Type
        {
            None,
            Obstacle,
        }
        public enum State
        {
            None,
            Start,
            Middle,
            End,
        }
        public Type type;
        public State state;
        public int g, h;
        public int f { get { return g + h; } }
    }
}
