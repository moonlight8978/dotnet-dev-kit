// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

using HybridModelBinding;
using Microsoft.AspNetCore.Mvc;

namespace DotnetDevelopmentSdk.Lib.Api;

public interface IControllerActionHandler<TRequest, TResponse>
    where TRequest : class, new() where TResponse : class, new()
{
    Task<ActionResult<HttpResponseData<TResponse>>> Perform([FromHybrid] HttpRequestData<TRequest> requestData);
}
