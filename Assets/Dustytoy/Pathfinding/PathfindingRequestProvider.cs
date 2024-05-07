using System;
using System.Threading;
using Dustytoy.Collections;
using Dustytoy.DI;

namespace Dustytoy.Pathfinding
{
    public class PathfindingRequestProvider : IPathfindingRequestProvider
    {
        [Inject]
        public IObjectPool<PathfindingRequest> requestPool { get; set; }
        [Inject]
        public IOpenListProvider openListProvider { get; set; }
        [Inject]
        public IClosedListProvider closedListProvider { get; set; }

        public (IDisposable,IPathfindingRequest) New(INode start, INode end, CancellationToken cancellationToken = default)
        {
            var (openListHandle, openList) = openListProvider.New();
            var (closedListHandle, closedList) = closedListProvider.New();
            var requestHandle = requestPool.Acquire(
                req=>
                {
                    req.Initialize(
                        start, end,
                        openList,
                        closedList,
                        cancellationToken);
                }, 
                req=>
                {
                    req.Clean();
                    openListHandle.Dispose();
                    closedListHandle.Dispose();
                });
            return (requestHandle, requestHandle.item);
        }
    }
}
