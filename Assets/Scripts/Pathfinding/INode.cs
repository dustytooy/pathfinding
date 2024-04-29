using System;

namespace Pathfinding
{
    public interface INode : IEquatable<INode>, IComparable<INode>
    {
        public bool isObstacle { get; set; }
        public int gCost { get; set; }
        public int hCost { get; set; }
        public int fCost { get; }
        public INode parent { get; set; }
    }
}
