namespace Pathfinding
{
    internal class OpenList
    {
        public int count { get { return nodes.count; } }
        public MinHeap<INode> nodes { get; private set; }

        public OpenList(int nodeInitialCapacity)
        {
            nodes = new MinHeap<INode>(nodeInitialCapacity);
        }

        public void Push(INode node) => nodes.Insert(node);
        public INode Pop() => nodes.ExtractMin();
        public void MinHeapify(INode node)
        {
            int index = nodes.list.IndexOf(node);
            nodes.MinHeapify(index);
        }
        public bool Contains(INode node) => nodes.list.Contains(node);
    }
}
