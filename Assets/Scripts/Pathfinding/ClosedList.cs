using System.Collections.Generic;

namespace Pathfinding
{
    internal class ClosedList<T> : IClosedList<T> where T : INode
    {
        public List<T> nodes { get; private set; }
        public bool Empty => nodes.Count == 0;
        public int Count => nodes.Count;
        public int Capacity => nodes.Capacity;

        public ClosedList()
        {
            nodes = new List<T>();
        }

        public T Min()
        {
            return nodes[0];
        }

        public void Add(T node)
        {
            if (node == null) throw new System.ArgumentNullException(typeof(T).Name);

            int index = Count;
            nodes.Add(node);

            while (index != 0 && nodes[index].CompareTo(nodes[Parent(index)]) <= 0)
            {
                Swap(index, Parent(index));
                index = Parent(index);
            }
        }

        public bool Contains(T node)
        {
            if (node == null) throw new System.ArgumentNullException(typeof(T).Name);
            return nodes.Contains(node);
        }

        private void Swap(int index1, int index2)
        {
            T temp = nodes[index1];
            nodes[index1] = nodes[index2];
            nodes[index2] = temp;

        }
        private int Parent(int index)
        {
            return (index - 1) / 2;
        }
    }
}