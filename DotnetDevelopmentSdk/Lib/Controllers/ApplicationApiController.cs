// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

using System.Security.Claims;
using DotnetDevelopmentSdk.Lib.Api;
using HybridModelBinding;
using Microsoft.AspNetCore.Mvc;

namespace DotnetDevelopmentSdk.Lib.Controllers;

[ApiController]
public class ApplicationApiController : ControllerBase
{
    private string? _currentUserIdCache;

    protected virtual string CurrentUserId
    {
        get
        {
            _currentUserIdCache ??=
                HttpContext.User.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value;
            return _currentUserIdCache;
        }
    }

    protected IActionResult MakeResponse<T>(IServiceReturnValue<T> serviceReturnValue, bool wrap = true) where T : class
    {
        if (serviceReturnValue.Code == ServiceReturnValue.Success)
        {
            return wrap
                ? Ok(new HttpResponseData<T> { Data = serviceReturnValue.Data })
                : Ok(serviceReturnValue.Data);
        }

        return BadRequest(
            new ErrorResponse { Code = serviceReturnValue.Code, Message = serviceReturnValue.FailReason, });
    }
}

public interface IControllerActionHandler<TRequest, TResponse>
{
    Task<ActionResult<HttpResponseData<TResponse>>> Perform([FromHybrid] HttpRequestData<TRequest> requestData);
}

public class HttpResponseData<T>
{
    public T Data { get; set; } = default!;
}

public class HttpRequestData<T>
{
    public T Data { get; set; } = default!;
}

public class ErrorResponse
{
    public int Code { get; set; }
    public string Message { get; set; } = "";
}
