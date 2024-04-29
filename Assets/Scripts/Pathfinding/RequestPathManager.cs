namespace Pathfinding
{
    public static class RequestPathManager
    {
        public static T[] ResolvePath<T>(ref IGraph<T> graph) where T : INode
        {
            graph.openList.Push(graph.start);

            while(!graph.openList.Empty)
            {
                var q = graph.openList.Pop();
                var neighbors = graph.GetNeighbors(q);

                foreach(var n in neighbors)
                {
                    n.parent = q;
                    
                    if(n.Equals(graph.end))
                    {
                        break;
                    }

                    int f_prev = n.fCost;

                    n.gCost = graph.CalculateGCost(n,q);
                    n.hCost = graph.CalculateHCost(n);
                    
                    if(graph.openList.Contains(n) && n.fCost < f_prev)
                    {
                        continue;
                    }

                    if(graph.closedList.Contains(n) && n.fCost < f_prev)
                    {
                        continue;
                    }

                    graph.openList.Push(n);
                }

                graph.closedList.Add(q);
            }

            return graph.closedList.result;
        }
    }
}
