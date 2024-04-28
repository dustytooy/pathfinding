using System.Collections.Generic;
using UnityEngine;

namespace Pathfinding
{
    public class RequestPathManager
    {
        private int _heapCapacity;
        private INodeFinder _nodeFinder;
        private ICostCalculator _costCalculator;

        public RequestPathManager(
            INodeFinder nodeConverter,
            ICostCalculator costCalculator,
            int heapCapacity)
        {
            _nodeFinder = nodeConverter;
            _costCalculator = costCalculator;
            _heapCapacity = heapCapacity;
        }

        public PathResult RequestPath(PathRequest request)
        {
            var start = _nodeFinder.GetNode(request.start);
            var end = _nodeFinder.GetNode(request.end);

            var openList = new OpenList(_heapCapacity);
            var closedList = new List<INode>();

            openList.Push(start);
            while(openList.count > 0)
            {
                var q = openList.Pop();
                var neighbors = _nodeFinder.GetNeighbors(q);

                foreach(var n in neighbors)
                {
                    n.parent.Value = q;
                    
                    if(n == end)
                    {
                        break;
                    }

                    int g = _costCalculator.G(n, q);
                    int h = _costCalculator.H(n, end);
                    int f = g + h;
                    
                    if(openList.Contains(n) && n.fCost < f)
                    {
                        continue;
                    }

                    if(closedList.Contains(n) && n.fCost < f)
                    {
                        continue;
                    }

                    openList.Push(n);
                }

                closedList.Add(q);
            }

            var waypoints = new Vector2[closedList.Count];
            for(int i = 0; i < waypoints.Length; i++)
            {
                waypoints[i] = _nodeFinder.GetPosition(closedList[i]);
            }
            var result = new PathResult();
            result.success = true;
            result.waypoints = waypoints;
            return result;
        }
    }
}
