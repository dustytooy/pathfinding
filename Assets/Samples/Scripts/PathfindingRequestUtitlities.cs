using System;
using UniRx;
using Dustytoy.Pathfinding;

namespace Dustytoy.Samples
{
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

    public static class PathfindingRequestUtilities
    {
        public static IObservable<StreamData> ProcessStream(this IPathfindingRequest request)
        {
            if (request.isDone)
            {
                throw new InvalidOperationException("Request is already done!");
            }
            request.CancellationToken.ThrowIfCancellationRequested();

            return Observable.Create<StreamData>(observer =>
            {
                request.pathfindingStatus = PathfindingStatus.InProgress;

                var start = request.start;
                var end = request.end;

                request.openList.Add(start);
                observer.OnNext(new StreamData(NodeAction.Start, start));
                while (!request.openList.isEmpty && !request.CancellationToken.IsCancellationRequested)
                {
                    var cur = request.openList.Pop();
                    var neighbors = cur.GetNeighbors();

                    foreach (var n in neighbors)
                    {
                        if (n.Equals(end))
                        {
                            n.parent = cur;
                            request.result = n.Traceback();
                            request.pathfindingStatus = PathfindingStatus.PathFound;
                            request.isDone = true;
                            observer.OnNext(new StreamData(NodeAction.End, n));
                            observer.OnCompleted();
                            return Disposable.Empty;
                        }
                        if (!request.closedList.Contains(n))
                        {
                            int gNew = n.CalculateGCost(cur);
                            int hNew = n.CalculateHCost();
                            int fNew = gNew + hNew;

                            if (!request.openList.Contains(n) || n.fCost > fNew)
                            {
                                n.gCost = gNew;
                                n.hCost = hNew;
                                n.parent = cur;
                                request.openList.Add(n);
                                observer.OnNext(new StreamData(NodeAction.AddToOpenList, n));
                            }
                        }
                    }

                    request.closedList.Add(cur);
                    observer.OnNext(new StreamData(NodeAction.AddToClosedList, cur));
                }

                if (request.CancellationToken.IsCancellationRequested)
                {
                    request.error = new OperationCanceledException(request.CancellationToken);
                    observer.OnError(request.error);
                    return Disposable.Empty;
                }
                request.result = request.closedList.Peek().Traceback();
                request.pathfindingStatus = PathfindingStatus.PathNotFound;
                request.isDone = true;
                observer.OnCompleted();
                return Disposable.Empty;
            });
        }

        public static IObservable<StreamData> ProcessStream(this IPathfindingRequest request, IObservable<Unit> source)
        {
            if (request.isDone)
            {
                throw new InvalidOperationException("Request is done");
            }
            if (request.CancellationToken.IsCancellationRequested)
            {
                throw request.error = new OperationCanceledException(request.CancellationToken);
            }

            return Observable.Create<StreamData>(observer =>
            {
                request.pathfindingStatus = PathfindingStatus.InProgress;

                var start = request.start;
                var end = request.end;

                bool addedStartNode = false;

                return source.Subscribe(
                x =>
                {
                    if (!addedStartNode && !request.CancellationToken.IsCancellationRequested)
                    {
                        request.openList.Add(start);
                        addedStartNode = true;
                        observer.OnNext(new StreamData(NodeAction.Start, start));
                    }
                    else
                    {
                        if (!request.openList.isEmpty && !request.CancellationToken.IsCancellationRequested)
                        {
                            var cur = request.openList.Pop();
                            var neighbors = cur.GetNeighbors();

                            foreach (var n in neighbors)
                            {
                                if (n.Equals(end))
                                {
                                    n.parent = cur;
                                    request.result = n.Traceback();
                                    request.pathfindingStatus = PathfindingStatus.PathFound;
                                    request.isDone = true;
                                    observer.OnNext(new StreamData(NodeAction.End, n));
                                    observer.OnCompleted();
                                    return;
                                }
                                if (!request.closedList.Contains(n))
                                {
                                    int gNew = n.CalculateGCost(cur);
                                    int hNew = n.CalculateHCost();
                                    int fNew = gNew + hNew;

                                    if (!request.openList.Contains(n) || n.fCost > fNew)
                                    {
                                        n.gCost = gNew;
                                        n.hCost = hNew;
                                        n.parent = cur;
                                        request.openList.Add(n);
                                        observer.OnNext(new StreamData(NodeAction.AddToOpenList, n));
                                    }
                                }
                            }
                            request.closedList.Add(cur);
                            observer.OnNext(new StreamData(NodeAction.AddToClosedList, cur));
                        }
                        else
                        {
                            if (request.CancellationToken.IsCancellationRequested)
                            {
                                request.error = new OperationCanceledException(request.CancellationToken);
                                observer.OnError(request.error);
                                return;
                            }
                            request.result = request.closedList.Peek().Traceback();
                            request.pathfindingStatus = PathfindingStatus.PathNotFound;
                            request.isDone = true;
                            observer.OnCompleted();
                        }
                    }
                },
                observer.OnError,
                () =>
                {
                // If wait source is completed, also cancel this stream
                    request.error = new OperationCanceledException();
                    observer.OnError(request.error);
                });
            }
            );
        }
    }
}
