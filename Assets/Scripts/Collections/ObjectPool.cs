using System;
using System.Collections.Generic;

namespace Dustytoy.Collections
{
    public struct ObjectPoolHandle<T> where T : new()
    {
        public T value;
        private ObjectPool<T> _pool;
        private Action<T> _onReleased;
        internal ObjectPoolHandle(T value, ObjectPool<T> pool,
            Action<T> onReleased = null)
        {
            this.value = value;
            _pool = pool;
            _onReleased = onReleased;
        }

        public void Release()
        {
            _pool.Release(value);
            _onReleased?.Invoke(value);
        }
    }
    public class ObjectPool<T> where T : new()
    {
        public List<T> elements { get; private set; }

        public ObjectPool()
        {
            elements = new List<T>();
        }

        public ObjectPool(int initialCapacity = 0)
        {
            elements = new List<T>(initialCapacity);
        }

        public ObjectPoolHandle<T> Acquire(Action<T> onReleased = null)
        {
            int count = elements.Count;
            if (count == 0)
            {
                T newValue = new T();
                return new ObjectPoolHandle<T>(newValue, this, onReleased);
            }
            T pooledValue = elements[count - 1];
            elements.RemoveAt(count - 1);
            return new ObjectPoolHandle<T>(pooledValue, this, onReleased);
        }
        internal void Release(T element)
        {
            elements.Add(element);
        }

    }
}
