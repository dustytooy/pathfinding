namespace Pathfinding
{
    public interface IGraph<T> where T : INode
    {
        public T start { get; set; }
        public T end { get; set; }

        public IOpenList<T> openList { get; set; }
        public IClosedList<T> closedList { get; set; }

        public T[] GetNeighbors(T node);

        public int CalculateGCost(T current, T parent);
        public int CalculateHCost(T current);
    }
}
