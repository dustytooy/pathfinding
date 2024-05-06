using System;
using System.Collections.Generic;
using System.Reflection;

namespace Dustytoy.DI
{
    public class DIContainer
    {
        public static readonly InvalidOperationException DifferentLifetimeException = new InvalidOperationException("Can not register with different lifetime!");
        public static readonly InvalidOperationException SingletonAlreadyExistsException = new InvalidOperationException("Can not register singleton with more than one value!");
        public static readonly InvalidOperationException AbstractInterfaceAsImplementation = new InvalidOperationException("Can not use abstract or interface as implementation!");
        public static readonly KeyNotFoundException KeyNotFoundException = new KeyNotFoundException();

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

        private readonly IDictionary<Type, Value> _container = new Dictionary<Type, Value>();

        public void Register<TContract, TImplementation>(Lifetime lifetime) where TImplementation : new()
        {
            var type = typeof(TContract);
            if(!_container.TryGetValue(type, out Value val))
            {
                var implType = typeof(TImplementation);
                if(implType.IsAbstract || implType.IsInterface)
                {
                    throw AbstractInterfaceAsImplementation;
                }
                val = new Value(lifetime, new TImplementation());
                _container.Add(type, val);
            }
            else
            {
                if (lifetime != val.Lifetime)
                {
                    throw DifferentLifetimeException;
                }
                if (lifetime == Lifetime.Singleton)
                {
                    throw SingletonAlreadyExistsException;
                }
            }
        }
        public void RegisterAsSingleton<TContract, TImplementation>() where TImplementation : new() => Register<TContract, TImplementation>(Lifetime.Singleton);
        public void RegisterAsTransient<TContract, TImplementation>() where TImplementation : new() => Register<TContract, TImplementation>(Lifetime.Transient);
        public void Register<T>(T instance, Lifetime lifetime) where T : new()
        {
            var type = typeof(T);
            if(!_container.TryGetValue(type, out Value val))
            {
                val = new Value(lifetime, instance);
                _container.Add(type, val);
            }
            else
            {
                if (lifetime != val.Lifetime)
                {
                    throw DifferentLifetimeException;
                }
                if(lifetime == Lifetime.Singleton)
                {
                    throw SingletonAlreadyExistsException;
                }
            }
        }
        public void RegisterAsSingleton<T>(T instance) where T : new() => Register<T>(instance, Lifetime.Singleton);
        public void RegisterAsTransient<T>(T instance) where T : new() => Register<T>(instance, Lifetime.Transient);

        public bool Unregister<T>()
        {
            return _container.Remove(typeof(T));
        }
        public void UnregisterAll()
        {
            _container.Clear();
        }

        public object Resolve(Type type)
        {
            if (_container.TryGetValue(type, out Value value))
            {
                return value.Instance;
            }
            throw KeyNotFoundException;
        }
        public T Resolve<T>() => (T)Resolve(typeof(T));

        public void Inject(object target)
        {
            // Get all fields of target object
            var fields = target.GetType()
                .GetFields(BindingFlags.Static | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

            foreach(var field in fields)
            {
                // Only target fields with [Inject] attribute
                var injectAttributes = field.GetCustomAttributes(typeof(InjectAttribute), true);

                if (injectAttributes.Length <= 0)
                {
                    continue;
                }

                try
                {
                    field.SetValue(target, Resolve(field.GetType()));
                }
                catch (KeyNotFoundException)
                {
                    throw KeyNotFoundException;
                }

                if(!_container.TryGetValue(field.FieldType, out Value value))
                {
                    continue;
                }
            }
        }
    }
}
