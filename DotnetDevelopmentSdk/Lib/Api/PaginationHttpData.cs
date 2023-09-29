// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

using Swashbuckle.AspNetCore.Annotations;

namespace DotnetDevelopmentSdk.Lib.Api;

public interface IPaginationRequestData
{
    int PerPage { get; set; }
    int Page { get; set; }
}

public interface IPaginationResponseData
{
    PaginationInfo Pagination { get; set; }
}

public class PaginationRequestData : IPaginationRequestData
{
    public int PerPage { get; set; }
    public int Page { get; set; }
}

public class PaginationInfo
{
    [SwaggerSchema(Nullable = false)] public int TotalPages { get; set; }

    [SwaggerSchema(Nullable = false)] public int CurrentPage { get; set; }

    [SwaggerSchema(Nullable = false)] public int PerPage { get; set; }

    [SwaggerSchema(Nullable = false)] public int TotalItems { get; set; }
}
