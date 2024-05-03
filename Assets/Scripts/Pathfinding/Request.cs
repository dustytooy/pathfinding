using System;
using System.Collections;
using System.Threading;
using Dustytoy.Collections;

namespace Dustytoy.Pathfinding
{
    internal class OpenList : MinHeap<INode> { }
    internal class ClosedList : MinHeap<INode> { }
    public class Request
    {
        public static void InitializePool()
        {
            // GC alloc
            openListPool = new ObjectPool<OpenList>();
            closedListPool = new ObjectPool<ClosedList>();
        }
        public static void CleanPool()
        {
            // Allow GC to clean
            openListPool = null;
            closedListPool = null;
        }
        private static ObjectPool<OpenList> openListPool;
        private static ObjectPool<ClosedList> closedListPool;

        public enum PathfindingStatus
        {
            Initialized,
            InProgress,
            PathNotFound,
            PathFound,
        }

        public bool isDone { get; private set; }
        public INode[] result { get; private set; }
        public PathfindingStatus pathfindingStatus { get; private set; }
        public Exception error { get; private set; }

        private OpenList openList;
        private ObjectPoolHandle<OpenList> openListHandle;
        private ClosedList closedList;
        private ObjectPoolHandle<ClosedList> closedListHandle;

        private INode start;
        private INode end;
        private CancellationToken token;

        public void Initialize(INode start, INode end, CancellationToken cancellationToken = default)
        {
            this.start = start;
            this.end = end;
            openListHandle = openListPool.Acquire(x=>x.Clear());
            closedListHandle = closedListPool.Acquire(x => x.Clear());

            isDone = false;
            result = null;
            error = null;
            pathfindingStatus = PathfindingStatus.Initialized;

            token = cancellationToken;
        }

        public void Clean()
        {
            start = null;
            end = null;

            closedListHandle.Release();
            openListHandle.Release();
            openList = null;
            closedList = null;
        }

        public PathfindingStatus Process(
            Action<INode> addToOpenListCallback = null,
            Action<INode> addToClosedListCallback = null)
        {
            pathfindingStatus = PathfindingStatus.InProgress;
            openList = openListHandle.value;
            closedList = closedListHandle.value;
            openList.Add(start);

            while (!openList.IsEmpty() && !token.IsCancellationRequested)
            {
                var cur = openList.Pop();
                var neighbors = cur.GetNeighbors();

                foreach (var n in neighbors)
                {
                    if (n.Equals(end))
                    {
                        n.parent = cur;
                        result = n.Traceback();
                        isDone = true;
                        return PathfindingStatus.PathFound;
                    }
                    if (!closedList.Contains(n))
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

            if (!token.IsCancellationRequested)
            {
                error = new OperationCanceledException(token);
                isDone = true;
                return PathfindingStatus.PathNotFound;
            }
            result = closedList.Peek().Traceback();
            isDone = true;
            return PathfindingStatus.PathNotFound;
        }

        public IEnumerator ProcessCoroutine(
            Action<INode> addToOpenListCallback = null,
            Action<INode> addToClosedListCallback = null,
            IEnumerator waitYieldInstruction = null)
        {
            pathfindingStatus = PathfindingStatus.InProgress;
            openList = openListHandle.value;
            closedList = closedListHandle.value;
            openList.Add(start);

            while (!openList.IsEmpty() && !token.IsCancellationRequested)
            {
                var cur = openList.Pop();
                var neighbors = cur.GetNeighbors();

                foreach (var n in neighbors)
                {
                    if (n.Equals(end))
                    {
                        n.parent = cur;
                        result = n.Traceback();
                        isDone = true;
                        yield break;
                    }
                    if (!closedList.Contains(n))
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
                yield return waitYieldInstruction;
            }

            if (!token.IsCancellationRequested)
            {
                error = new OperationCanceledException(token);
                isDone = true;
                yield break;
            }
            result = closedList.Peek().Traceback();
            isDone = true;
        }
    }
}
