// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

using DotnetDevelopmentSdk.Lib.Pagination;

namespace DummyApp.Features.Pagination;

public class PaginationRequestData : IPaginationRequestData
{
    public int Page { get; set; }
    public int PerPage { get; set; }
    public int? From { get; set; }
    public int? To { get; set; }
}
