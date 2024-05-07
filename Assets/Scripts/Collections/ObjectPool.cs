using System;
using System.Collections.Generic;

namespace Dustytoy.Collections
{
    public interface IObjectPool<T> where T : new()
    {
        public ObjectPoolItemHandle<T> Acquire();
        public ObjectPoolItemHandle<T> Acquire(Action<T> onAcquired = null, Action<T> onReleased = null);
        public void Release(T value);
    }

    public struct ObjectPoolItemHandle<T> : IDisposable where T : new()
    {
        public T item;
        private IObjectPool<T> _pool;
        private Action<T> _onReleased;

        public ObjectPoolItemHandle(T item, IObjectPool<T> pool, Action<T> onReleased = null)
        {
            this.item = item;
            _pool = pool;
            _onReleased = onReleased;
        }

        public void Dispose()
        {
            _pool.Release(item);
            _onReleased.Invoke(item);
        }
    }

    public class ObjectPool<T> : IObjectPool<T> where T : new()
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

        public ObjectPoolItemHandle<T> Acquire()
        {
            int count = elements.Count;
            if (count == 0)
            {
                T newValue = new T();
                return new ObjectPoolItemHandle<T>(newValue, this);
            }
            var pooledValue = elements[count - 1];
            elements.RemoveAt(count - 1);
            return new ObjectPoolItemHandle<T>(pooledValue, this);
        }

        public ObjectPoolItemHandle<T> Acquire(Action<T> onAcquired = null, Action<T> onReleased = null)
        {
            int count = elements.Count;
            if (count == 0)
            {
                T newValue = new T();
                onAcquired?.Invoke(newValue);
                return new ObjectPoolItemHandle<T>(newValue, this, onReleased);
            }
            var pooledValue = elements[count - 1];
            elements.RemoveAt(count - 1);
            onAcquired?.Invoke(pooledValue);
            return new ObjectPoolItemHandle<T>(pooledValue, this, onReleased);
        }

        public void Release(T element)
        {
            elements.Add(element);
        }
    }
}
