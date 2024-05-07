using System;
using System.Collections.Generic;

namespace Dustytoy.Collections
{
    public interface IObjectPool<T> where T : IObjectPoolItem
    {
        public ObjectPoolItemHandle Acquire();
        public ObjectPoolItemHandle Acquire(Action<IObjectPoolItem> onAcquired = null, Action<IObjectPoolItem> onReleased = null);
        public void Release(T value);
    }

    public interface IObjectPoolItem
    {
        public object instance { get; set; }
    }

    public struct ObjectPoolItemHandle : IDisposable
    {
        public IObjectPoolItem item;
        private IObjectPool<IObjectPoolItem> _pool;
        private Action<IObjectPoolItem> _onReleased;

        public ObjectPoolItemHandle(IObjectPoolItem item, IObjectPool<IObjectPoolItem> pool, Action<IObjectPoolItem> onReleased = null)
        {
            this.item = item;
            _pool = pool;
            _onReleased = onReleased;
        }

        public void Dispose()
        {
            _pool.Release(item);
            _onReleased?.Invoke(item);
        }
    }

    public class ObjectPool<T> : IObjectPool<T> where T : IObjectPoolItem, new()
    {
        public List<IObjectPoolItem> elements { get; private set; }
        private IObjectPool<IObjectPoolItem> _self;

        public ObjectPool()
        {
            _self = this as IObjectPool<IObjectPoolItem>;
            elements = new List<IObjectPoolItem>();
        }

        public ObjectPool(int initialCapacity = 0)
        {
            _self = this as IObjectPool<IObjectPoolItem>;
            elements = new List<IObjectPoolItem>(initialCapacity);
        }

        public ObjectPoolItemHandle Acquire()
        {
            int count = elements.Count;
            if (count == 0)
            {
                T newValue = new T();
                return new ObjectPoolItemHandle(newValue, _self);
            }
            var pooledValue = elements[count - 1];
            elements.RemoveAt(count - 1);
            return new ObjectPoolItemHandle(pooledValue, _self);
        }

        public ObjectPoolItemHandle Acquire(Action<IObjectPoolItem> onAcquired = null, Action<IObjectPoolItem> onReleased = null)
        {
            int count = elements.Count;
            if (count == 0)
            {
                T newValue = new T();
                onAcquired?.Invoke(newValue);
                return new ObjectPoolItemHandle(newValue, _self, onReleased);
            }
            var pooledValue = elements[count - 1];
            elements.RemoveAt(count - 1);
            onAcquired?.Invoke(pooledValue);
            return new ObjectPoolItemHandle(pooledValue, _self, onReleased);
        }

        public void Release(T element)
        {
            elements.Add(element);
        }
    }
}
