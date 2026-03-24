namespace GeneticAlgorithm.ExtensionMethods;

public static class EnumerableExtensions
{
    public static IEnumerable<TSource> TakeMax<TSource, TKey>(this IEnumerable<TSource> source,
        Func<TSource, TKey> keySelector, int n)
    {
        if (n < 0)
        {
            throw new ArgumentException(nameof(n));
        }
        
        if (n == 0)
        {
            return Enumerable.Empty<TSource>();
        }
        

        var pq = new PriorityQueue<TSource, TKey>();
        foreach (var s in source)
        {
            TKey key = keySelector(s);
            if (pq.Count < n)
            {
                pq.Enqueue(s, key);
            }
            else
            {
                pq.EnqueueDequeue(s, key);
            }
        }

        return pq.UnorderedItems.Select(q => q.Element);
    }
}