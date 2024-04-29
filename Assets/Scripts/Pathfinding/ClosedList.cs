using System.Collections.Generic;

namespace Pathfinding
{
    internal class ClosedList<T> : IClosedList<T> where T : INode
    {
        public List<T> nodes { get; private set; }

        public T[] result => nodes.ToArray();

        public ClosedList()
        {
            nodes = new List<T>();
        }

        public void Add(T node)
        {
            if (node == null) throw new System.ArgumentNullException(typeof(T).Name);
            nodes.Add(node);
        }

        public bool Contains(T node)
        {
            if (node == null) throw new System.ArgumentNullException(typeof(T).Name);
            return nodes.Contains(node);
        }
    }
}