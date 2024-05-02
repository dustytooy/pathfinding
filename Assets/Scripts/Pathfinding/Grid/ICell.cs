using UnityEngine;

namespace Dustytoy.Pathfinding.Grid
{
    public interface ICell : INode
    {
        public Vector2Int position { get; }
    }
}
