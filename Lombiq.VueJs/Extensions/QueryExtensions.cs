using OrchardCore.Settings;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace YesSql;

public static class QueryExtensions
{
    public static async Task<object> GetPageFromQueryAsync<T>(
        this IQuery<T> query,
        ISiteService siteService,
        int page,
        Func<T, object> select)
        where T : class
    {
        var total = await query.CountAsync();
        var (pageSize, pageCount) = await siteService.GetPaginationInfoAsync(total);
        return new
        {
            items = (await query.PaginateAsync(page, pageSize)).Select(select),
            pageSize,
            pageCount,
        };
    }
}
