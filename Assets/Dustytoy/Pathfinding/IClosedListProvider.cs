using System;

namespace Dustytoy.Pathfinding
{
    public interface IOpenListProvider
    {
        public (IDisposable, IOpenList) New();
    }
}
