// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

using DotnetDevelopmentSdk.Lib.Utils;
using Microsoft.EntityFrameworkCore;

namespace DotnetDevelopmentSdk.Lib.Pagination;

public class PaginationService : ITypeDirectedTransientBindedService
{
    private int _page = 1;
    private int _perPage = 20;
    private int _totalPages = 1;
    private int _totalItems;
    private int _offset;
    private int _from;
    private int _to;
    private bool _isRange;

    public async Task<IQueryable<T>> PerformAsync<T>(IQueryable<T> queryable)
    {
        _totalItems = await queryable.CountAsync();

        if (IsRangeUsed())
        {
            _from = Math.Min(_totalItems, _from);
            _offset = _from;
            return queryable.Skip(_offset).Take(_to - _from + 1);
        }

        _totalPages = (int)Math.Ceiling((double)_totalItems / _perPage);
        return queryable.Skip((_page - 1) * _perPage)
            .Take(_perPage);
    }

    public IEnumerable<T> PerformAsync<T>(IEnumerable<T> queryable)
    {
        var items = queryable.ToList();
        _totalItems = items.Count;

        if (IsRangeUsed())
        {
            _from = Math.Min(_totalItems, _from);
            _offset = _from;
            return items.Skip(_offset).Take(_to - _from + 1);
        }

        _totalPages = (int)Math.Ceiling((double)_totalItems / _perPage);
        return items.Skip((_page - 1) * _perPage).Take(_perPage);
    }

    public void Configure(Action<PaginationService> configureDefaultOptions)
    {
        configureDefaultOptions.Invoke(this);
    }

    public void SetPaginationInfoFromRequest(IPaginationRequestData paginationRequestData)
    {
        _page = Math.Max(paginationRequestData.Page, 1);
        _perPage = Math.Min(paginationRequestData.PerPage, 1000);
        _from = (_page - 1) * _perPage;
        _to = _page * _perPage - 1;

        if (paginationRequestData.From.HasValue && paginationRequestData.To.HasValue)
        {
            _isRange = true;
            _from = Math.Max(0, paginationRequestData.From.Value);
            _to = Math.Max(1, Math.Min(_from + 5000, paginationRequestData.To.Value));
        }
    }

    public void SetResponseData<T>(T responseData) where T : IPaginationResponseData
    {
        responseData.Page = _page;
        responseData.PerPage = _perPage;
        responseData.Total = _totalItems;
        responseData.IsLastPage = _page >= _totalPages;
        responseData.From = _from;
        responseData.To = _to;
    }

    private bool IsRangeUsed()
    {
        return _isRange;
    }
}
