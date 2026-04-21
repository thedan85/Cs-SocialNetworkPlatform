using System.Linq;

namespace SocialNetwork.Extensions;

public static class QueryableExtensions
{
    public const int DefaultPageSize = 20;
    public const int MaxPageSize = 100;

    public static IQueryable<T> ApplyPaging<T>(
        this IQueryable<T> query,
        int pageNumber,
        int pageSize,
        int defaultPageSize = DefaultPageSize,
        int maxPageSize = MaxPageSize)
    {
        var validatedPageNumber = pageNumber < 1 ? 1 : pageNumber;
        var validatedPageSize = pageSize < 1 ? defaultPageSize : pageSize;

        if (validatedPageSize > maxPageSize)
        {
            validatedPageSize = maxPageSize;
        }

        return query
            .Skip((validatedPageNumber - 1) * validatedPageSize)
            .Take(validatedPageSize);
    }
}
