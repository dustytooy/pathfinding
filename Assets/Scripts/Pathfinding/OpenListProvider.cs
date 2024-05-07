using System;
using Dustytoy.Collections;
using Dustytoy.DI;

namespace Dustytoy.Pathfinding
{
    public class OpenListProvider : IOpenListProvider
    {
        [Inject]
        public IObjectPool<OpenList> openListPool { get; set; }

        public (IDisposable, IOpenList) New()
        {
            var openListHandle = openListPool.Acquire(null, x => x.Clear());
            return (openListHandle, openListHandle.item);
        }
    }
}
