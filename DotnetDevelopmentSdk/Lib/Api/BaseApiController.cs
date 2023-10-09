// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

using Microsoft.AspNetCore.Mvc;

namespace DotnetDevelopmentSdk.Lib.Api;

[ApiController]
public abstract class BaseApiController : ControllerBase
{
    protected IActionResult MakeResponse<T>(IServiceReturnValue<T> serviceReturnValue, bool wrap = true) where T : class, new()
    {
        if (serviceReturnValue.Code == ServiceReturnValue.Success)
        {
            return wrap
                ? Ok(new HttpResponseData<T> { Data = serviceReturnValue.Data })
                : Ok(serviceReturnValue.Data);
        }

        return BadRequest(
            new ErrorResponse() { Code = serviceReturnValue.Code, Message = serviceReturnValue.FailReason, });
    }

    protected ActionResult<T> MakeResponse<T>(ApiActionServiceResult apiActionServiceResult) where T : HttpResponseData
    {
        if (apiActionServiceResult.IsSuccess())
        {
            return Ok(apiActionServiceResult.ResponseData);
        }

        return BadRequest(apiActionServiceResult.ErrorData);
    }
}

public class HttpResponseData
{
    public object Data { get; set; } = new();
}

public class HttpResponseData<T> : HttpResponseData where T : class, new()
{
    public new T Data { get; set; } = new();
}

public class HttpRequestData
{
    public object Data { get; set; } = new();
}

public class HttpRequestData<T> : HttpRequestData where T : class, new()
{
    public new T Data { get; set; } = new();
}

public class HttpErrorResponseData
{
    public int Code { get; set; }
    public int Status { get; set; }
    public string Message { get; set; } = string.Empty;
    public List<string> Messages { get; set; } = new();
    public object Data { get; set; } = new();
}

public class HttpErrorResponseData<T> : HttpErrorResponseData where T : class, new()
{
    public new T Data { get; set; } = new();
}
