using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Pathfinding
{
    // From https://egorikas.com/max-and-min-heap-implementation-with-csharp/
    internal class MinHeap
    {
        //private readonly INode[] _elements;
        private readonly List<INode> _elements;
        private int _size;

        public MinHeap(int size)
        {
            _elements = new List<INode>(size);
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

        private INode GetLeftChild(int elementIndex) => _elements[GetLeftChildIndex(elementIndex)];
        private INode GetRightChild(int elementIndex) => _elements[GetRightChildIndex(elementIndex)];
        private INode GetParent(int elementIndex) => _elements[GetParentIndex(elementIndex)];

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

        public INode Peek()
        {
            if (_size == 0)
                throw new IndexOutOfRangeException();

            return _elements[0];
        }

        public INode Pop()
        {
            if (_size == 0)
                throw new IndexOutOfRangeException();

            var result = _elements[0];
            _elements[0] = _elements[_size - 1];
            _size--;

            ReCalculateDown();

            return result;
        }

        public void Add(INode element)
        {
            //if (_size == _elements.Count)
            //    throw new IndexOutOfRangeException();

            //_elements[_size] = element;
            _elements.Add(element);
            _size++;
            ReCalculateUp();
        }

        public bool Contains(INode element, int subtreeIndex = 0)
        {
            if (_size == 0)
                return false;

            if (_elements[subtreeIndex].Equals(element))
            {
                return true;
            }
            var leftContains = false;
            if (HasLeftChild(subtreeIndex))
            {
                var left = GetLeftChild(subtreeIndex);
                if (left.CompareTo(element) > 0)
                {
                    leftContains = Contains(element, GetLeftChildIndex(subtreeIndex));
                }
            }
            if (leftContains)
            {
                return true;
            }
            var rightContains = false;
            if (HasRightChild(subtreeIndex))
            {
                var right = GetRightChild(subtreeIndex);
                if (right.CompareTo(element) > 0)
                {
                    rightContains = Contains(element, GetRightChildIndex(subtreeIndex));
                }
            }
            if (rightContains)
            {
                return true;
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
