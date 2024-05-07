using Dustytoy.Collections;

namespace Dustytoy.Pathfinding
{
    public interface IPathfindingRequestPoolable : IPathfindingRequest, IObjectPoolItem
    {
    }
    public class PathfindingRequestPoolable : PathfindingRequest, IPathfindingRequestPoolable
    {
        public object instance { get; set; }
        public PathfindingRequestPoolable()
        {
            instance = this;
        }
    }
    public interface IOpenListPoolable : IOpenList, IObjectPoolItem
    {
    }
    public class OpenListPoolable : OpenList, IObjectPoolItem
    {
        public object instance { get; set; }
        public OpenListPoolable()
        {
            instance = this;
        }
    }
    public interface IClosedListPoolable : IClosedList, IObjectPoolItem
    {
    }
    public class ClosedListPoolable : ClosedList, IObjectPoolItem
    {
        public object instance { get; set; }
        public ClosedListPoolable()
        {
            instance = this;
        }
    }
}
