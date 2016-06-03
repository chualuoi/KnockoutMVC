using System;
using System.Collections.Generic;

namespace KoLib.Mvc.ValidationInfrastructure.Helpers
{
    public static class CollectionHelpers
    {
        public static bool IsCollection(this object obj)
        {
            return (obj != null)
                && (obj.GetType() != typeof(String))
                && (typeof(System.Collections.IEnumerable).IsInstanceOfType(obj));
        }

        public static int CollectionGetCount(this object collection)
        {
            if (!collection.IsCollection())
                throw new Exception("Object is not a collection type!");

            if (collection.GetType().IsArray)
                return ((Array)collection).GetLength(0);
            return (int)collection.GetType().GetProperty("Count")
                            .GetValue(collection, null);
        }

        public static object CollectionGetItem(this object collection, int index)
        {
            if (!collection.IsCollection())
                throw new Exception("Object is not a collection type!");
            if (index >= CollectionGetCount(collection) || index < 0)
                throw new ArgumentOutOfRangeException("Index is greater than length of current collection!");

            if (collection.GetType().IsArray)
                return ((Array)collection).GetValue(index);
            return collection.GetType().GetProperty("Item")
                .GetValue(collection, new object[] { index });
        }

        public static void CollectionSetItem(this object collection, int index, object value)
        {
            if (!collection.IsCollection())
                throw new Exception("Object is not a collection type!");
            if (index >= CollectionGetCount(collection) || index < 0)
                throw new ArgumentOutOfRangeException("Index is greater than length of current collection!");

            if (collection.GetType().IsArray)
                ((Array)collection).SetValue(value, index);
            else
                collection.GetType().GetProperty("Item")
                    .SetValue(collection, value, new object[] { index });
        }

        public static List<object> ToListObject(this object collection)
        {
            if (!collection.IsCollection())
            {
                return null;
            }
            var result = new List<object>();
            for (int i = 0; i < collection.CollectionGetCount(); i++)
            {
                result.Add(collection.CollectionGetItem(i));
            }
            return result;
        }
    }
}