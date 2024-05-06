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

        internal void Initialize(INode start, INode end, ObjectPoolHandle<OpenList> openListHandle, ObjectPoolHandle<ClosedList> closedListHandle, CancellationToken cancellationToken = default)
        {
            this.start = start;
            this.end = end;
            this.openListHandle = openListHandle;
            this.closedListHandle = closedListHandle;
            openList = openListHandle.value;
            closedList = closedListHandle.value;

            isDone = false;
            result = null;
            error = null;
            pathfindingStatus = PathfindingStatus.Initialized;

            token = cancellationToken;
        }

        internal void Clean()
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
            if (isDone)
            {
                throw new InvalidOperationException("Request is already done");
            }
            if (token.IsCancellationRequested)
            {
                throw error = new OperationCanceledException(token);
            }

            pathfindingStatus = PathfindingStatus.InProgress;

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

        public IObservable<StreamData> ProcessStream()
        {
            if(isDone)
            {
                throw new InvalidOperationException("Request is already done!");
            }
            if (token.IsCancellationRequested)
            {
                throw error = new OperationCanceledException(token);
            }

            return Observable.Create<StreamData> (observer =>
            {
                pathfindingStatus = PathfindingStatus.InProgress;

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

                if (token.IsCancellationRequested)
                {
                    error = new OperationCanceledException(token);
                    observer.OnError(error);
                    return Disposable.Empty;
                }
                result = closedList.Peek().Traceback();
                pathfindingStatus = PathfindingStatus.PathNotFound;
                isDone = true;
                observer.OnCompleted();
                return Disposable.Empty;
            });
        }

        public IObservable<StreamData> ProcessStream(IObservable<Unit> source)
        {
            if (isDone)
            {
                throw new InvalidOperationException("Request is done");
            }
            if (token.IsCancellationRequested)
            {
                throw error = new OperationCanceledException(token);
            }

            return Observable.Create<StreamData>(observer =>
            {
                pathfindingStatus = PathfindingStatus.InProgress;
                bool addedStartNode = false;

                return source.Subscribe(
                x =>
                {
                    if (!addedStartNode && !token.IsCancellationRequested)
                    {
                        openList.Add(start);
                        addedStartNode = true;
                        observer.OnNext(new StreamData(NodeAction.Start, start));
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
                                    observer.OnNext(new StreamData(NodeAction.End, n));
                                    observer.OnCompleted();
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
                                        observer.OnNext(new StreamData(NodeAction.AddToOpenList, n));
                                    }
                                }
                            }
                            closedList.Add(cur);
                            observer.OnNext(new StreamData(NodeAction.AddToClosedList, cur));
                        }
                        else
                        {
                            if (token.IsCancellationRequested)
                            {
                                error = new OperationCanceledException(token);
                                observer.OnError(error);
                                return;
                            }
                            result = closedList.Peek().Traceback();
                            pathfindingStatus = PathfindingStatus.PathNotFound;
                            isDone = true;
                            observer.OnCompleted();
                        }
                    }
                },
                observer.OnError,
                () =>
                    {
                        // If wait source is completed, also cancel this stream
                        error = new OperationCanceledException();
                        observer.OnError(error);
                    });
                }
            );
        }
    }
}
