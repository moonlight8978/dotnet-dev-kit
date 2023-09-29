// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

using DotnetDevelopmentSdk.Lib.Utils;
using Microsoft.EntityFrameworkCore;

namespace DotnetDevelopmentSdk.Lib.Api;

public class PaginationService : ITypeDirectedScopeBindedService
{
    private int _page;
    private int _perPage;
    private int _totalPages;
    private int _totalItems;
    private readonly PaginationDefaultOptions _defaultOptions = new();

    public void Configure(Action<PaginationDefaultOptions> configureDefaultOptions)
    {
        configureDefaultOptions.Invoke(_defaultOptions);
    }

    public void SetPaginationInfoFromRequest(IPaginationRequestData paginationRequestData)
    {
        _page = paginationRequestData.Page > 0 ? paginationRequestData.Page : _defaultOptions.Page;
        _perPage = paginationRequestData.PerPage > 0 && paginationRequestData.PerPage <= _defaultOptions.MaxPerPage
            ? paginationRequestData.PerPage
            : _defaultOptions.PerPage;
    }

    public void SetPaginationInfoToResponse(IPaginationResponseData paginationResponseData)
    {
        paginationResponseData.Pagination = new PaginationInfo
        {
            CurrentPage = _page,
            PerPage = _perPage,
            TotalItems = _totalItems,
            TotalPages = _totalPages
        };
    }

    public async Task<IQueryable<T>> PerformAsync<T>(IQueryable<T> queryable)
    {
        _totalItems = await queryable.CountAsync();
        _totalPages = (int)Math.Ceiling((double)_totalItems / _perPage);

        return queryable.Skip((_page - 1) * _perPage)
            .Take(_perPage);
    }

    public IEnumerable<T> PerformAsync<T>(IEnumerable<T> queryable)
    {
        var items = queryable.ToList();
        _totalItems = items.Count;
        _totalPages = (int)Math.Ceiling((double)_totalItems / _perPage);

        return items.Skip((_page - 1) * _perPage).Take(_perPage);
    }
}

public class PaginationDefaultOptions
{
    public int PerPage { get; set; } = 20;
    public int Page { get; set; } = 1;
    public int MaxPerPage { get; set; } = 50;
}
