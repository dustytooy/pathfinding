using Dustytoy.Collections;

namespace Dustytoy.Pathfinding
{
    public class OpenList : MinHeap<INode>, IOpenList
    {
        public bool isEmpty => IsEmpty();

        public bool Contains(INode node) => Contains(node);
    }
}
