using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Dustytoy.Collections;

namespace Dustytoy.Pathfinding
{
    public static class PathfindingManager
    {
        private static ObjectPool<Request> requestPool;

        public static bool Initialized { get; private set; }
        public static void InitializePool()
        {
            // GC alloc
            requestPool = new ObjectPool<Request>();
            Pathfinding.Request.InitializePool();
            Initialized = true;
        }
        public static void CleanPool()
        {
            // Allow GC to clean
            requestPool = null;
            Pathfinding.Request.CleanPool();
            Initialized = false;
        }

        public static ObjectPoolHandle<Request> Request(INode start, INode end, CancellationToken cancellationToken = default)
        {
            var request = requestPool.Acquire(x=>x.Clean());
            request.value.Initialize(start, end, cancellationToken);
            return request;
        }
    }
}
