using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Dustytoy.Collections
{
    // From https://egorikas.com/max-and-min-heap-implementation-with-csharp/
    public class MinHeap<T> where T : IComparable<T>
    {
        public static readonly int DefaultInitialCapacity = 10;
        //private readonly T[] _elements;
        private readonly List<T> _elements;
        private int _size;

        public MinHeap()
        {
            _elements = new List<T>(DefaultInitialCapacity);
        }
        public MinHeap(int initialCapacity)
        {
            _elements = new List<T>(initialCapacity);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int GetLeftChildIndex(int elementIndex) => 2 * elementIndex + 1;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int GetRightChildIndex(int elementIndex) => 2 * elementIndex + 2;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int GetParentIndex(int elementIndex) => (elementIndex - 1) / 2;

        private bool HasLeftChild(int elementIndex) => GetLeftChildIndex(elementIndex) < _size;
        private bool HasRightChild(int elementIndex) => GetRightChildIndex(elementIndex) < _size;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool IsRoot(int elementIndex) => elementIndex == 0;

        private T GetLeftChild(int elementIndex) => _elements[GetLeftChildIndex(elementIndex)];
        private T GetRightChild(int elementIndex) => _elements[GetRightChildIndex(elementIndex)];
        private T GetParent(int elementIndex) => _elements[GetParentIndex(elementIndex)];

        private void Swap(int firstIndex, int secondIndex)
        {
            var temp = _elements[firstIndex];
            _elements[firstIndex] = _elements[secondIndex];
            _elements[secondIndex] = temp;
        }

        public bool IsEmpty()
        {
            return _size == 0;
        }

        public void Clear(int setCapacity)
        {
            _size = 0;
            _elements.Clear();
            _elements.Capacity = setCapacity;
        }
        public void Clear() => Clear(DefaultInitialCapacity);

        public T Peek()
        {
            if (_size == 0)
                throw new IndexOutOfRangeException();

            return _elements[0];
        }

        public T Pop()
        {
            if (_size == 0)
                throw new IndexOutOfRangeException();

            var result = _elements[0];
            _elements[0] = _elements[_size - 1];
            _elements.RemoveAt(_size - 1);
            _size--;
            ReCalculateDown();

            return result;
        }

        public void Add(T element)
        {
            //if (_size == _elements.Count)
            //    throw new IndexOutOfRangeException();

            //_elements[_size] = element;
            _elements.Add(element);
            _size++;
            ReCalculateUp();
        }

        public bool Contains(T element, int subtreeIndex = 0)
        {
            if (_size == 0)
                return false;
            //return _elements.Contains(element);
            if (_elements[subtreeIndex].Equals(element))
            {
                return true;
            }

            // Check left
            if (HasLeftChild(subtreeIndex) && GetLeftChild(subtreeIndex).CompareTo(element) <= 0)
            {
                if(Contains(element, GetLeftChildIndex(subtreeIndex)))
                {
                    return true;
                }
            }
            // Check right
            if (HasRightChild(subtreeIndex) && GetRightChild(subtreeIndex).CompareTo(element) <= 0)
            {
                if (Contains(element, GetRightChildIndex(subtreeIndex)))
                {
                    return true;
                }
            }
            return false;
        }

        private void ReCalculateDown()
        {
            int index = 0;
            while (HasLeftChild(index))
            {
                var smallerIndex = GetLeftChildIndex(index);
                if (HasRightChild(index) && GetRightChild(index).CompareTo(GetLeftChild(index)) < 0)
                {
                    smallerIndex = GetRightChildIndex(index);
                }

                if (_elements[smallerIndex].CompareTo(_elements[index]) >= 0)
                {
                    break;
                }

                Swap(smallerIndex, index);
                index = smallerIndex;
            }
        }

        private void ReCalculateUp()
        {
            var index = _size - 1;
            while (!IsRoot(index) && _elements[index].CompareTo(GetParent(index)) <= 0)
            {
                var parentIndex = GetParentIndex(index);
                Swap(parentIndex, index);
                index = parentIndex;
            }
        }
    }
}
