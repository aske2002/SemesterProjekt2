namespace tremorur.Utils;
public static class ICollectionExtensions
{
    public static IEnumerable<E> TakeWhileCompare<E>(this IEnumerable<E> collection, Func<(E current, E next), bool> predicate)
    {
        using var enumerator = collection.GetEnumerator();
        if (!enumerator.MoveNext())
            yield break;

        var current = enumerator.Current;
        yield return current;

        while (enumerator.MoveNext())
        {
            var next = enumerator.Current;
            if (!predicate((current, next)))
                yield break;

            yield return next;
            current = next;
        }
    }
}