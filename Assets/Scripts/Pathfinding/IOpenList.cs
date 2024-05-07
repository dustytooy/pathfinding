namespace Dustytoy.Pathfinding
{
    public interface IOpenList
    {
        public bool isEmpty { get; }
        public void Add(INode node);
        public INode Pop();
        public bool Contains(INode node);
        public void Clear();
    }
}
