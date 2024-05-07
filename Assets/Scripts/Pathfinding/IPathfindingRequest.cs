using System;
using System.Threading;

namespace Dustytoy.Pathfinding
{
    public enum PathfindingStatus
    {
        Initialized,
        InProgress,
        PathNotFound,
        PathFound,
    }
    public interface IPathfindingRequest
    {
        public bool isDone { get; set; }
        public CancellationToken CancellationToken { get; }
        public INode[] result { get; set; }
        public PathfindingStatus pathfindingStatus { get; set; }
        public IOpenList openList { get; }
        public IClosedList closedList { get; }
        public INode start { get; }
        public INode end { get; }
        public Exception error { get; set; }
        public void Process();
        public void Initialize(INode start, INode end, IOpenList openList, IClosedList closedList, CancellationToken cancellationToken = default);
        public void Clean();
    }
}
