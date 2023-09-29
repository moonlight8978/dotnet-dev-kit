// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

using DotnetDevelopmentSdk.Lib.Exceptions;
using RestSharp;

namespace DotnetDevelopmentSdk.Lib.Services.HttpClient;

public class HttpRequestException : BaseException
{
    public RestResponseBase Response { get; }

    public HttpRequestException(RestResponseBase response, Exception innerException = null) : base(
        $"HTTP Request failed {response.StatusCode}", innerException)
    {
        PublicMessage = "Something went wrong with our internal system.";
        Response = response;
    }

    public override bool ShouldBeLogged => true;
}
