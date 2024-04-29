namespace Pathfinding
{
    public interface IOpenList<T> where T : INode
    {
        public bool Empty { get; }
        public void Push(T node);
        public T Pop();
        public bool Contains(T node);
    }
}
