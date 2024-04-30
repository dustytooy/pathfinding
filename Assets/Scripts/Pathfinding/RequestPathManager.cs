using System;

namespace Pathfinding
{
    public static class RequestPathManager
    {
        public enum Status
        {
            PathFound,
            Pathfinding,
            PathNotFound,
        }
        public static (Status, TNode[]) ResolvePath<TNode>(
            IGraph<TNode> graph, 
            Action<TNode> addToOpenListCallback = null,
            Action<TNode> addToClosedListCallback = null) where TNode : INode
        {
            graph.openList.Push(graph.start);

            while (!graph.openList.Empty)
            {
                var q = graph.openList.Pop();
                var neighbors = graph.GetNeighbors(q);

                foreach(var n in neighbors)
                {
                    if(graph.IsEnd(n))
                    {
                        n.parent = q;
                        graph.final = n;
                        return (Status.PathFound, graph.TraceBack());
                    }
                    if (!graph.closedList.Contains(n))
                    {
                        int gNew = graph.CalculateGCost(n, q);
                        int hNew = graph.CalculateHCost(n);
                        int fNew = gNew + hNew;

                        if (!graph.openList.Contains(n) || n.fCost > fNew)
                        {
                            n.gCost = gNew;
                            n.hCost = hNew;
                            n.parent = q;
                            graph.openList.Push(n);
                            addToOpenListCallback?.Invoke(n);
                        }
                    }
                }

                graph.closedList.Add(q);
                addToClosedListCallback?.Invoke(q);
            }
            graph.final = graph.closedList.Min();
            return (Status.PathNotFound, graph.TraceBack());
        }


    }
}
