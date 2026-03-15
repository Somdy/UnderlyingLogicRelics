using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;

namespace UnderlyingLogicRelics.Frameworks.Utils
{
    public static class Extensions
    {
        public static void RemoveAll<T>(this ICollection<T> collection, Func<T, bool> match)
        {
            var items = collection.Where(match).ToList();
            items.ForEach(item => collection.Remove(item));
        }

        public static IEnumerable<T> RemoveAll<T>(this IEnumerable<T> collection, Func<T, bool> match)
        {
            var items = collection.Where(t => !match(t)).ToList();
            return items;
        }

        [Obsolete]
        public static void SetPropertyValue(this object obj, Type type, string propertyName, object?[]? parameters)
        {
            PropertyInfo pi = AccessTools.Property(type, propertyName);
            MethodInfo setter = pi.GetSetMethod(nonPublic: true);
            if (setter != null)
            {
                setter.Invoke(obj, parameters);
                return;
            }

            FieldInfo backingField = pi.DeclaringType?.GetField($"<{pi.Name}>k__BackingField", 
                BindingFlags.Instance | BindingFlags.NonPublic);
            if (backingField == null)
            {
                throw new InvalidOperationException($"Unable to set property {propertyName} of type {type.Name}");
            }
            backingField.SetValue(obj, parameters);
        }
    }
}