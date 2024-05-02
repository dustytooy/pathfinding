using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Dustytoy.Collections;

namespace Dustytoy.Pathfinding
{
    public static class RequestPathManager
    {
        //public static ObjectPool<Request> requestPool;
        //public static ObjectPool<MinHeap<INode>> heapPool;
        //public static ObjectPool<List<INode>> addedToOpenListPool;

        //public static bool Initialized { get; private set; }
        //public static void Initialize()
        //{
        //    requestPool = ObjectPool<Request>.Instance;
        //    heapPool = ObjectPool<MinHeap<INode>>.Instance;
        //    addedToOpenListPool = ObjectPool<List<INode>>.Instance;

        //    Initialized = true;
        //}
        //public static void Clean()
        //{
        //    ObjectPool<Request>.Release();
        //    ObjectPool<MinHeap<INode>>.Release();
        //    ObjectPool<List<INode>>.Release();
        //    requestPool = null;
        //    heapPool = null;
        //    addedToOpenListPool = null;

        //    Initialized = false;
        //}

        //public static IEnumerator Single(
        //    INode start,
        //    INode end,
        //    IEnumerator<long> wait,
        //    CancellationToken cancellationToken)
        //{
        //    if (!Initialized) yield break;

        //    var req = requestPool.Acquire();

        //    req.Initialize(start, end, wait, cancellationToken);
        //    yield return req.Get();
        //    requestPool.Release(req);
        //}

        public static Request.Status RequestPathSimple(
            INode start, 
            INode end, 
            out INode[] path, 
            int heapCapacity,
            Action<INode> addToOpenListCallback = null,
            Action<INode> addToClosedListCallback = null)
        {
            var openList = new MinHeap<INode>(heapCapacity);
            var closedList = new MinHeap<INode>(heapCapacity);

            openList.Add(start);
            while (!openList.IsEmpty())
            {
                var cur = openList.Pop();
                var neighbors = cur.GetNeighbors();

                foreach (var n in neighbors)
                {
                    if (n.Equals(end))
                    {
                        n.parent = cur;
                        path = n.Traceback();
                        return Request.Status.PathFound;
                    }
                    if (!closedList. Contains(n))
                    {
                        int gNew = n.CalculateGCost(cur);
                        int hNew = n.CalculateHCost();
                        int fNew = gNew + hNew;

                        if (!openList.Contains(n) || n.fCost > fNew)
                        {
                            n.gCost = gNew;
                            n.hCost = hNew;
                            n.parent = cur;
                            openList.Add(n);
                            addToOpenListCallback?.Invoke(n);
                        }
                    }
                }

                closedList.Add(cur);
                addToClosedListCallback?.Invoke(cur);
            }
            path = closedList.Peek().Traceback();
            return Request.Status.PathNotFound;
        }
    }
}
