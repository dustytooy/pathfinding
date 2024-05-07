using System;
using System.Threading;

namespace Dustytoy.Pathfinding
{
    public class PathfindingRequest : IPathfindingRequest
    {
        public bool isDone { get; set; }
        public CancellationToken CancellationToken { get; private set; }
        public INode[] result { get; set; }
        public PathfindingStatus pathfindingStatus { get; set; }
        public IOpenList openList { get; private set; }
        public IClosedList closedList { get; private set; }
        public INode start { get; private set; }
        public INode end { get; private set; }
        public Exception error { get; set; }

        public void Initialize(INode start, INode end, IOpenList openList, IClosedList closedList, CancellationToken cancellationToken = default)
        {
            this.start = start;
            this.end = end;
            this.openList = openList;
            this.closedList = closedList;

            isDone = false;
            result = null;
            error = null;
            pathfindingStatus = PathfindingStatus.Initialized;

            CancellationToken = cancellationToken;
        }

        public void Clean()
        {
            start = null;
            end = null;

            openList = null;
            closedList = null;
        }

        public void Process()
        {
            if (isDone)
            {
                throw new InvalidOperationException("Request is already done");
            }
            CancellationToken.ThrowIfCancellationRequested();

            pathfindingStatus = PathfindingStatus.InProgress;

            openList.Add(start);
            while (!openList.isEmpty && !CancellationToken.IsCancellationRequested)
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
            CancellationToken.ThrowIfCancellationRequested();
            result = closedList.Peek().Traceback();
            pathfindingStatus = PathfindingStatus.PathNotFound;
            isDone = true;
        }
    }
}
