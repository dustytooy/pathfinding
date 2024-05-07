using System;
using System.Threading;

namespace Dustytoy.Pathfinding
{
    public interface IPathfindingRequestProvider
    {
        public IOpenListProvider openListProvider { get; }
        public IClosedListProvider closedListProvider { get; }

        public (IDisposable, IPathfindingRequest) New(INode start, INode end, CancellationToken cancellationToken = default);
    }
}
