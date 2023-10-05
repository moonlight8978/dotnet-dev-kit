// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

namespace DotnetDevelopmentSdk.Lib.Pagination;

public interface IPaginationResponseData
{
    public int Page { get; set; }
    public int PerPage { get; set; }
    public int Total { get; set; }
    public bool IsLastPage { get; set; }
    public int From { get; set; }
    public int To { get; set; }
}
