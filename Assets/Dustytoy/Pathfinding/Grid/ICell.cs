using Dustytoy.Collections;

namespace Dustytoy.Pathfinding.Grid
{
    public interface ICell : INode, ICanBeObstacle, IGrid2DItem, IHas2DPosition
    {
    }
}
