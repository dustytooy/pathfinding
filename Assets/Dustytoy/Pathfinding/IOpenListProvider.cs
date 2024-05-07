using System;

namespace Dustytoy.Pathfinding
{
    public interface IClosedListProvider
    {
        public (IDisposable, IClosedList) New();
    }
}
