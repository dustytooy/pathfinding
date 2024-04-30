namespace Pathfinding
{
    public interface IClosedList<T> where T : INode
    {
        public T Min();
        public void Add(T node);
        public bool Contains(T node);
    }
}
