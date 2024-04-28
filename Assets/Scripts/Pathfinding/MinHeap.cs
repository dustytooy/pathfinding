using System.Collections.Generic;

namespace Pathfinding
{
	internal class MinHeap<T> where T : INode
	{
		public List<T> list { get; private set; }
		public int count { get { return list.Count; } }
		public int capacity { get { return list.Capacity; } }

		public MinHeap(int n)
		{
			list = new List<T>(n);
		}
		
		public void Swap(int index1, int index2)
        {
			T temp = list[index1];
			list[index1] = list[index2];
			list[index2] = temp;
			
        }
		public int Parent(int index)
		{
			return (index - 1) / 2;
		}

		public int Left(int index)
		{
			return 2 * index + 1;
		}

		public int Right(int index)
		{
			return 2 * index + 2;
		}

		public void Insert(T value)
		{
			int index = count;
			list.Add(value);

			while (index != 0 && list[index].fCost < list[Parent(index)].fCost)
			{
				Swap(index, Parent(index));
				index = Parent(index);
			}
		}

		public T GetMin()
		{
			return list[0];
		}

		public T ExtractMin()
		{
			if (count == 0)
			{
				return default(T);
			}

			if (count == 1)
			{
				T min = list[0];
				list.RemoveAt(0);
				return min;
			}

			T root = list[0];

			list[0] = list[count - 1];
			list.RemoveAt(count - 1);
			MinHeapify(0);

			return root;
		}

		public void MinHeapify(int index)
		{
			while (index != 0 && list[index].fCost < list[Parent(index)].fCost)
			{
				Swap(index, Parent(index));
				index = Parent(index);
			}

			int l = Left(index);
			int r = Right(index);

			int smallest = index;
			if (l < count && list[l].fCost < list[smallest].fCost)
			{
				smallest = l;
			}
			if (r < count && list[r].fCost < list[smallest].fCost)
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
