// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

namespace DotnetDevelopmentSdk.Lib.Api;

public class ApiActionServiceResult
{
    public int Code { get; set; }
    public HttpResponseData? ResponseData { get; set; }
    public HttpErrorResponseData? ErrorData { get; set; }

    public bool IsSuccess()
    {
        return Code == CommonApiCode.Success;
    }
}
