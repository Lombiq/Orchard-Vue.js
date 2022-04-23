using Lombiq.VueJs.Models;
using OrchardCore.Settings;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace YesSql;

public static class QueryExtensions
{
    public static async Task<GetPageFromQueryResult> GetPageFromQueryAsync<T>(
        this IQuery<T> query,
        ISiteService siteService,
        int page,
        Func<T, object> select)
        where T : class
    {
        var total = await query.CountAsync();
        var (pageSize, pageCount) = await siteService.GetPaginationInfoAsync(total);
        return new GetPageFromQueryResult
        {
            Items = (await query.PaginateAsync(page, pageSize)).Select(select),
            PageSize = pageSize,
            PageCount = pageCount,
        };
    }
}
