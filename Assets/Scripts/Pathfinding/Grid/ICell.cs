using UnityEngine;

namespace Dustytoy.Pathfinding.Grid
{
    public interface IHasGridPosition : INode
    {
        public Vector2Int position { get; }
    }
}
