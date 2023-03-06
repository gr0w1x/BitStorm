namespace CommonServer.Utils.Extensions;

public static class QueryableExtensions
{
    public static IQueryable<T> SkipAndTake<T>(this IQueryable<T> query, int? skip, int? take)
    {
        if (skip != null)
        {
            query = query.Skip(skip.Value);
        }
        if (take != null)
        {
            query = query.Take(take.Value);
        }
        return query;
    }
}
