using System.Threading;
using Dustytoy.Collections;
using Dustytoy.DI;

namespace Dustytoy.Pathfinding
{
    public class PathfindingService : IPathfindingService
    {
        [Inject]
        public IObjectPool<IPathfindingRequestPoolable> requestPool { get; set; }
        [Inject]
        public IObjectPool<IOpenListPoolable> openListPool { get; set; }
        [Inject]
        public IObjectPool<IClosedListPoolable> closedListPool { get; set; }

        public (ObjectPoolItemHandle,IPathfindingRequest) Request(INode start, INode end, CancellationToken cancellationToken = default)
        {
            var openListHandle = openListPool.Acquire(x => (x as IOpenList).Clear());
            var closedListHandle = closedListPool.Acquire(x => (x as IClosedList).Clear());
            var request = requestPool.Acquire(
                req=>
                {
                    (req as IPathfindingRequest).Initialize(
                        start, end,
                        openListHandle.item as IOpenList,
                        closedListHandle.item as IClosedList,
                        cancellationToken);
                }, 
                req=>
                {
                    (req as IPathfindingRequest).Clean();
                    openListHandle.Dispose();
                    closedListHandle.Dispose();
                });
            return (request, request.item.instance as IPathfindingRequest);
        }
    }
}
