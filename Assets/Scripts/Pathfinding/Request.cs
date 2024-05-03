using System;
using System.Threading;
using Dustytoy.Collections;
using UniRx;

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

        public void Process()
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
                        pathfindingStatus = PathfindingStatus.PathFound;
                        isDone = true;
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
                        }
                    }
                }

                closedList.Add(cur);
            }

            if (!token.IsCancellationRequested)
            {
                error = new OperationCanceledException(token);
                pathfindingStatus = PathfindingStatus.PathNotFound;
                isDone = true;
            }
            result = closedList.Peek().Traceback();
            pathfindingStatus = PathfindingStatus.PathNotFound;
            isDone = true;
        }

        public enum NodeAction : int
        {
            Start,
            End,
            AddToOpenList,
            AddToClosedList,
        }

        public struct StreamData
        {
            public NodeAction action;
            public INode node;
            public StreamData(NodeAction action, INode node)
            {
                this.action = action;
                this.node = node;
            }
        }

        public IDisposable ProcessStream(Action<StreamData> onNext = null, Action<Exception> onError = null, Action onComplete = null)
        {
            return Observable.Create<StreamData> (observer =>
            {
                pathfindingStatus = PathfindingStatus.InProgress;
                openList = openListHandle.value;
                closedList = closedListHandle.value;

                openList.Add(start);
                observer.OnNext(new StreamData(NodeAction.Start, start));
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
                            pathfindingStatus = PathfindingStatus.PathFound;
                            isDone = true;
                            observer.OnNext(new StreamData(NodeAction.End, n));
                            observer.OnCompleted();
                            return Disposable.Empty;
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
                                observer.OnNext(new StreamData(NodeAction.AddToOpenList, n));
                            }
                        }
                    }

                    closedList.Add(cur);
                    observer.OnNext(new StreamData(NodeAction.AddToClosedList, cur));
                }

                if (!token.IsCancellationRequested)
                {
                    error = new OperationCanceledException(token);
                    pathfindingStatus = PathfindingStatus.PathNotFound;
                    isDone = true;
                    observer.OnError(error);
                    return Disposable.Empty;
                }
                result = closedList.Peek().Traceback();
                pathfindingStatus = PathfindingStatus.PathNotFound;
                isDone = true;
                observer.OnCompleted();
                return Disposable.Empty;
            }).Subscribe(onNext, onError, onComplete);
        }

        public IDisposable ProcessStreamWaitable(IObservable<Unit> waitSource, Action<StreamData> onNext = null, Action<Exception> onError = null, Action onComplete = null)
        {
            pathfindingStatus = PathfindingStatus.InProgress;
            openList = openListHandle.value;
            closedList = closedListHandle.value;
            bool addedStartNode = false;
            Action<Unit> onWaitStream = _ =>
            {
                if (!addedStartNode)
                {
                    openList.Add(start);
                    addedStartNode = true;
                    onNext(new StreamData(NodeAction.Start, start));
                }
                else
                {
                    if (!openList.IsEmpty() && !token.IsCancellationRequested)
                    {
                        var cur = openList.Pop();
                        var neighbors = cur.GetNeighbors();

                        foreach (var n in neighbors)
                        {
                            if (n.Equals(end))
                            {
                                n.parent = cur;
                                result = n.Traceback();
                                pathfindingStatus = PathfindingStatus.PathFound;
                                isDone = true;
                                onNext(new StreamData(NodeAction.End, n));
                                onComplete();
                                return;
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
                                    onNext(new StreamData(NodeAction.AddToOpenList, n));
                                }
                            }
                        }
                        closedList.Add(cur);
                        onNext(new StreamData(NodeAction.AddToClosedList, cur));
                    }
                    else
                    {
                        if (!token.IsCancellationRequested)
                        {
                            error = new OperationCanceledException(token);
                            pathfindingStatus = PathfindingStatus.PathNotFound;
                            isDone = true;
                            onError(error);
                            return;
                        }
                        result = closedList.Peek().Traceback();
                        pathfindingStatus = PathfindingStatus.PathNotFound;
                        isDone = true;
                        onComplete();
                    }
                }
            };
            return waitSource.Subscribe(onWaitStream);
        }
    }
}
