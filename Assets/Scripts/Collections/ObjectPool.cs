using System;
using System.Collections.Generic;

namespace Dustytoy.Collections
{
    public class ObjectPool<T> where T : new()
    {
        public List<T> elements { get; private set; }
        public static readonly int DefaultInitialCapacity = 10;
        public static ObjectPool<T> Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ObjectPool<T>(DefaultInitialCapacity);
                }
                return _instance;
            }
        }
        public static void Release()
        {
            _instance = null;
        }
        private static ObjectPool<T> _instance;

        private ObjectPool(int initialCapacity)
        {
            elements = new List<T>(initialCapacity);
            _instance = this;
        }

        public T Acquire(Action<T> onAcquired = null)
        {
            int count = elements.Count;
            if (count == 0)
            {
                T newElement = new T();
                onAcquired?.Invoke(newElement);
                return newElement;
            }
            T pooledElement = elements[count - 1];
            elements.RemoveAt(count - 1);
            onAcquired?.Invoke(pooledElement);

            return pooledElement;
        }
        public void Release(T element, Action<T> onReleased = null)
        {
            elements.Add(element);
            onReleased?.Invoke(element);
        }

    }
}
