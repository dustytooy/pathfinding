namespace Dustytoy.Pathfinding
{
    public interface IClosedList
    {
        public void Add(INode node);
        public INode Pop();
        public INode Peek();
        public bool Contains(INode node);
        public void Clear();
    }
}
