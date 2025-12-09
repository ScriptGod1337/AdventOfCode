namespace Common;

public static class EnumerationUtils {
    
    public static IEnumerable<(T, T)> CreateTuple2<T>(this IList<T> list) {
        for (int i = 0; i < list.Count; i++)
            for (int j = i + 1; j < list.Count; j++)
                yield return (list[i], list[j]);
    }

}