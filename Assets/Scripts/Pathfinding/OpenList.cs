using System.Collections.Generic;

namespace Pathfinding
{
    internal class OpenList<T> : IOpenList<T> where T : INode
    {
        public List<T> nodes { get; private set; }
        public bool Empty => nodes.Count == 0;
        public int Count => nodes.Count;
        public int Capacity => nodes.Capacity;

        public OpenList()
        {
            nodes = new List<T>();
        }

        public void Push(T node)
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

        public T Pop()
        {
            if (Count == 0)
            {
                return default(T);
            }

            if (Count == 1)
            {
                T min = nodes[0];
                nodes.RemoveAt(0);
                return min;
            }

            T root = nodes[0];

            nodes[0] = nodes[Count - 1];
            nodes.RemoveAt(Count - 1);
            MinHeapify(0);

            return root;
        }

        public bool Contains(T node)
        {
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
        private int Left(int index)
        {
            return 2 * index + 1;
        }
        private int Right(int index)
        {
            return 2 * index + 2;
        }
        private void MinHeapify(int index)
        {
            int l = Left(index);
            int r = Right(index);

            int smallest = index;
            if (l < Count && nodes[l].CompareTo(nodes[smallest]) <= 0)
            {
                smallest = l;
            }
            if (r < Count && nodes[r].CompareTo(nodes[smallest]) <= 0)
            {
                smallest = r;
            }
            if (smallest != index)
            {
                Swap(index, smallest);
                MinHeapify(smallest);
            }
        }
    }
}
