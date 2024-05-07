using Dustytoy.Collections;

namespace Dustytoy.Pathfinding
{
    public class ClosedList : MinHeap<INode>, IClosedList
    {
        public bool Contains(INode node) => Contains(node, 0);
    }
}
