// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

namespace DotnetDevelopmentSdk.Lib.Pagination;

public interface IPaginationRequestData
{
    int Page { get; set; }
    int PerPage { get; set; }
    public int? From { get; set; }
    public int? To { get; set; }
}
