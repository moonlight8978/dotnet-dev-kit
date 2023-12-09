// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

namespace DotnetDevelopmentSdk.Lib.Utils;

public static class EnumerableUtils
{
    public static IEnumerable<(T, int)> WithIndex<T>(this IEnumerable<T> collection)
    {
        return collection.Select((item, index) => (item, index));
    }

    public static void ForEachWithIndex<T>(this IEnumerable<T> collection, Action<T, int> perItemExecution)
    {
        foreach (var (item, index) in collection.WithIndex())
        {
            perItemExecution.Invoke(item, index);
        }
    }

    public static IEnumerable<T> Randomize<T>(this IEnumerable<T> items, int count)
    {
        var list = items.ToList();

        if (list.Count == 0)
        {
            return new List<T>();
        }

        var totalItems = list.Count;
        var randomizer = new Random();
        var selectedItems = new HashSet<T>();
        while (selectedItems.Count < count)
        {
            var item = list.ElementAt(randomizer.Next(totalItems));
            selectedItems.Add(item);
        }

        return selectedItems;
    }

    public static Dictionary<TKey, TValue> Merge<TKey, TValue>(this Dictionary<TKey, TValue> target,
        Dictionary<TKey, TValue> source) where TKey : notnull
    {
        foreach (var (key, value) in source)
        {
            if (!target.TryAdd(key, value))
            {
                target[key] = value;
            }
        }

        return target;
    }

    public static List<T> RemoveIf<T>(this List<T> collection, Func<T, bool> predicate)
    {
        var removedItems = new List<T>();
        var removeIndexes = new List<int>();

        collection.ForEachWithIndex((item, index) =>
        {
            var shouldRemove = predicate.Invoke(item);

            if (shouldRemove)
            {
                removedItems.Add(item);
                removeIndexes.Add(index);
            }
        });

        removeIndexes.ForEach(collection.RemoveAt);

        return removedItems;
    }

    public static float SafeAverage<T>(this IEnumerable<T> collection, Func<T, float> evaluate)
    {
        var iEnumerable = collection.ToList();
        return iEnumerable.Any() ? iEnumerable.Average(evaluate) : 0;
    }

    public static float SafeAverage<T>(this IEnumerable<T> collection, Func<T, int> evaluate)
    {
        var iEnumerable = collection.ToList();
        return iEnumerable.Any() ? iEnumerable.Average(item => (float)evaluate(item)) : 0;
    }

    public static int SafeMin<T>(this IEnumerable<T> collection, Func<T, int> evaluate, int fallback = 0)
    {
        var iEnumerable = collection.ToList();
        return iEnumerable.Any() ? iEnumerable.Min(evaluate) : fallback;
    }
}
