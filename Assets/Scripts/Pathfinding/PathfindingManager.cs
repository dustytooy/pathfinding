using System.Threading;
using Dustytoy.Collections;

namespace Dustytoy.Pathfinding
{
    public class PathfindingManager
    {
        private ObjectPool<Request> requestPool;
        private ObjectPool<OpenList> openListPool;
        private ObjectPool<ClosedList> closedListPool;

        public PathfindingManager()
        {
            requestPool = new ObjectPool<Request>();
            openListPool = new ObjectPool<OpenList>();
            closedListPool = new ObjectPool<ClosedList>();
        }

        public (ObjectPoolHandle<Request>,Request) Request(INode start, INode end, CancellationToken cancellationToken = default)
        {
            var request = requestPool.Acquire(
                req=>req.Initialize(start, end, 
                openListPool.Acquire(x => x.Clear()), 
                closedListPool.Acquire(x => x.Clear()), 
                cancellationToken), 
                req=>req.Clean());
            return (request, request.value);
        }
    }
}
