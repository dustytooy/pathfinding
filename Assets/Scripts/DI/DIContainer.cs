using System;
using System.Collections.Generic;
using System.Reflection;

namespace Dustytoy.DI
{
    // Reference https://qiita.com/ogix/items/0e6e98a058a608cf712c
    public class DIContainer
    {
        public enum Lifetime
        {
            Singleton, // Always same instance
            Transient, // New instance
        }

        private class Value
        {
            public Lifetime Lifetime { get; set; }
            public object Instance { get; set; }
            public Value(Lifetime lifetime, object instance)
            {
                Lifetime = lifetime;
                Instance = instance;
            }
        }

        private readonly Dictionary<Type, Value> _container = new Dictionary<Type, Value>();

        private static readonly DIContainer _instance = new DIContainer();
        public static DIContainer Instance => _instance;

        public void Register<T>(Lifetime lifetime) where T : class, new()
        {
            var type = typeof(T);
            if(_container.TryGetValue(type, out Value value))
            {
                if(value.Lifetime == Lifetime.Singleton)
                {
                    return;
                }

                var ins = new T();
                _container.Add(type, new Value(lifetime, ins));
            }
        }

        public void Register<T>(T ins, Lifetime lifetime)
        {
            var type = typeof(T);
            if(_container.TryGetValue(type, out Value value))
            {
                if(value.Lifetime == Lifetime.Singleton)
                {
                    return;
                }

                value.Lifetime = lifetime;
                value.Instance = ins;
            }
            else
            {
                _container.Add(type, new Value(lifetime, ins));
            }
        }

        public void Unregister<T>() where T : class
        {
            _container.Remove(typeof(T));
        }

        public void UnregisterAll()
        {
            _container.Clear();
        }

        public T Resolve<T>() where T : class
        {
            if (_container.TryGetValue(typeof(T), out Value value))
            {
                return (T)value.Instance;
            }
            return default;
        }

        public void Inject(object target)
        {
            // Get all fields of target object
            var fields = target.GetType()
                .GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

            foreach(var field in fields)
            {
                // Only target fields with [Inject] attribute
                var injectAttributes = field.GetCustomAttributes(typeof(InjectAttribute), true);
                if (injectAttributes.Length <= 0)
                {
                    continue;
                }

                if(!_container.TryGetValue(field.FieldType, out Value value))
                {
                    continue;
                }

                // Inject dependency
                field.SetValue(target, value.Instance);
            }
        }
    }
}
