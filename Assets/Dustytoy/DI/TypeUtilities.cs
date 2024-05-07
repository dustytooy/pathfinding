using System;
using System.Linq.Expressions;
using System.Runtime.Serialization;

namespace Dustytoy.DI
{
    // Reference from https://stackoverflow.com/questions/6582259/fast-creation-of-objects-instead-of-activator-createinstancetype
    public static class TypeUtilities
    {
        public static class New<T>
        {
            public static readonly Func<T> Instance = Creator();

            static Func<T> Creator()
            {
                Type t = typeof(T);
                if (t == typeof(string))
                    return Expression.Lambda<Func<T>>(Expression.Constant(string.Empty)).Compile();

                if (t.HasDefaultConstructor())
                    return Expression.Lambda<Func<T>>(Expression.New(t)).Compile();

                return () => (T)FormatterServices.GetUninitializedObject(t);
            }
        }

        public static bool HasDefaultConstructor(this Type t)
        {
            return t.IsValueType || t.GetConstructor(Type.EmptyTypes) != null;
        }
    }
}
