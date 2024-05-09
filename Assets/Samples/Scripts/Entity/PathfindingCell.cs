using Dustytoy.Pathfinding.Grid;
using Dustytoy.Pathfinding;

namespace Dustytoy.Samples.Grid2D.Entity
{
    internal interface IPathfindingNode : INode, ICanBeObstacle
    {
    }
    /// <summary>
    /// A wrapper over Cell class in Pathfinding assembly
    /// </summary>
    internal class PathfindingCell : Cell, IPathfindingNode
    {
        public PathfindingCell(int x, int y, bool isObstacle, IGrid grid, int xGoalCoordinate, int yGoalCoordinate) : base(x, y, isObstacle, grid, xGoalCoordinate, yGoalCoordinate)
        {
        }
    }
}
