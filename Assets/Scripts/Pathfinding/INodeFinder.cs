using System.Collections.Generic;
using UnityEngine;

namespace Pathfinding
{
    public interface INodeFinder
    {
        public INode GetNode(Vector2 position);
        public Vector2 GetPosition(INode node);
        public List<INode> GetNeighbors(INode node);
    }
}
