using System;
using System.Collections.Generic;
using System.Collections;
using System.Threading;
using Dustytoy.Collections;

namespace Dustytoy.Pathfinding
{
    public class Request
    {
        public enum Status
        {
            PathNotFound,
            PathFound,
            InProgress,
        }

        //public class PerStepData
        //{
        //    public Status status;
        //    public INode[] addedToOpenList;
        //    public INode addedToClosedListOrGoal; // If In Progress, the node in closed list, if Path Found, the goal node
        //}

        //public Request() { }

        //public bool IsCompleted { get; private set; }
        //public INode[] Result { get; private set; }
        //public Exception Error { get; private set; }
        //public bool HasError => Error != null;

        //private MinHeap<INode> openList;
        //private MinHeap<INode> closedList;
        //private List<INode> addedToOpenList;

        //private INode start;
        //private INode end;
        //private IEnumerator<long> observable;
        //private CancellationToken cancellationToken;
        //private PerStepData perStepData = new PerStepData();

        //public void Initialize(
        //    INode start,
        //    INode end,
        //    IEnumerator<long> wait,
        //    CancellationToken cancellationToken)
        //{
        //    openList = RequestPathManager.heapPool.Acquire();
        //    closedList = RequestPathManager.heapPool.Acquire();
        //    addedToOpenList = RequestPathManager.addedToOpenListPool.Acquire();

        //    this.start = start;
        //    this.end = end;
        //    this.observable = wait;
        //    this.cancellationToken = cancellationToken;
            
        //    openList.Add(start);
        //}

        //public void Clean()
        //{
        //    start = null;
        //    end = null;
        //    observable = null;

        //    RequestPathManager.heapPool.Release(openList);
        //    RequestPathManager.heapPool.Release(closedList);
        //    RequestPathManager.addedToOpenListPool.Release(addedToOpenList);
        //    openList = null;
        //    closedList = null;
        //    addedToOpenList = null;
        //}

        //public IEnumerator Get()
        //{
        //    IsCompleted = false;
        //    Result = null;

        //    while (!openList.IsEmpty() && !cancellationToken.IsCancellationRequested)
        //    {
        //        var data = Step();
        //        if(data.status == Status.PathFound)
        //        {
        //            Result = data.addedToClosedListOrGoal.Traceback();
        //            IsCompleted = true;
        //        }

        //        yield return observable;
        //    }

        //    if (cancellationToken.IsCancellationRequested) yield break;

        //    perStepData.status = Status.PathNotFound;

        //    Result = closedList.Peek().Traceback();
        //    IsCompleted = true;

        //    yield return observable;
        //}

        //private PerStepData Step()
        //{
        //    addedToOpenList.Clear();

        //    var cur = openList.Pop();
        //    var neighbors = cur.GetNeighbors();
        //    foreach (var n in neighbors)
        //    {
        //        if (n.Equals(end))
        //        {
        //            n.parent = cur;

        //            perStepData.addedToOpenList = addedToOpenList.ToArray();
        //            perStepData.addedToClosedListOrGoal = n;
        //            perStepData.status = Status.PathFound;

        //            return perStepData;
        //        }
        //        if (!closedList.Contains(n))
        //        {
        //            int gNew = n.CalculateGCost(cur);
        //            int hNew = n.CalculateHCost();
        //            int fNew = gNew + hNew;

        //            if (!openList.Contains(n) || n.fCost > fNew)
        //            {
        //                n.gCost = gNew;
        //                n.hCost = hNew;
        //                n.parent = cur;
        //                openList.Add(n);
        //                addedToOpenList.Add(n);
        //            }
        //        }
        //    }

        //    closedList.Add(cur);

        //    perStepData.addedToOpenList = addedToOpenList.ToArray();
        //    perStepData.addedToClosedListOrGoal = cur;
        //    perStepData.status = Status.InProgress;
        //    return perStepData;
        //}
    }
}
