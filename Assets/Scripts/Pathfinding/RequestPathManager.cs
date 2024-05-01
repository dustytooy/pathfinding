using System;

namespace Pathfinding
{
    public static class RequestPathManager
    {
        public enum Status
        {
            PathFound,
            PathNotFound,
        }
        public static Status RequestPath(
            INode start, 
            INode end, 
            out INode[] path, 
            int heapCapacity,
            Action<INode> addToOpenListCallback = null,
            Action<INode> addToClosedListCallback = null)
        {
            var openList = new MinHeap(heapCapacity);
            var closedList = new MinHeap(heapCapacity);

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
                        return Status.PathFound;
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
            return Status.PathNotFound;
        }
    }
}
