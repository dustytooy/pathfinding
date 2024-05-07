using System;
using Dustytoy.Collections;
using Dustytoy.DI;

namespace Dustytoy.Pathfinding
{
    public class ClosedListProvider : IClosedListProvider
    {
        [Inject]
        public IObjectPool<ClosedList> closedListPool { get; set; }

        public (IDisposable, IClosedList) New()
        {
            var closedListHandle = closedListPool.Acquire(null, x => x.Clear());
            return (closedListHandle, closedListHandle.item);
        }
    }
}
