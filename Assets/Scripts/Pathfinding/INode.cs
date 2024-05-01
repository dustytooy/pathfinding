using System;

namespace Pathfinding
{
    public interface INode : IEquatable<INode>, IComparable<INode>
    {
        public int gCost { get; set; }
        public int hCost { get; set; }
        public int fCost { get; }
        public INode parent { get; set; }

        public INode[] GetNeighbors();
        public INode[] Traceback();
        public int CalculateGCost(INode from);
        public int CalculateHCost();
    }
}
