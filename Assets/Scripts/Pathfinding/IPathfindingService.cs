using System.Threading;
using Dustytoy.Collections;

namespace Dustytoy.Pathfinding
{
    public interface IPathfindingService
    {
        public IObjectPool<IPathfindingRequestPoolable> requestPool { get; }
        public IObjectPool<IOpenListPoolable> openListPool { get; }
        public IObjectPool<IClosedListPoolable> closedListPool { get; }

        public (ObjectPoolItemHandle, IPathfindingRequest) Request(INode start, INode end, CancellationToken cancellationToken = default);
    }
}
