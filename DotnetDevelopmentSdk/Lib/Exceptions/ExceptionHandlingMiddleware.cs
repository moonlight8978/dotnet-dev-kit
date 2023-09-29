// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

using DotnetDevelopmentSdk.Lib.Environment;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Serilog;

namespace DotnetDevelopmentSdk.Lib.Exceptions;

public abstract class BaseExceptionHandlingMiddleware : IMiddleware
{
    private readonly IWebHostEnvironment _environment;
    protected abstract ILogger Logger { get; }

    protected BaseExceptionHandlingMiddleware(IWebHostEnvironment environment)
    {
        _environment = environment;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }

        catch (BaseException baseException)
        {
            if (baseException.ShouldBeLogged)
            {
                Logger.Error(baseException, $"Request failed: User-defined error {baseException.PublicMessage}");
            }

            var errorDetails = OnUserDefinedError(context, baseException);
            await HandleErrorAsync(context, errorDetails);
        }
        catch (Exception unexpectedException)
        {
            Logger.Error(unexpectedException, $"Request failed: Unexpected error {unexpectedException.Message}");

            var errorDetails = OnUnexpectedError(context, unexpectedException);
            await HandleErrorAsync(context, errorDetails);
        }
    }

    protected record ErrorDetails
    {
        public int InternalCode { get; init; }
        public int StatusCode { get; init; }
        public string Message { get; init; }
    }

    protected virtual ErrorDetails OnUnexpectedError(HttpContext _, Exception ex)
    {
        return new ErrorDetails
        {
            Message = _environment.IsDevelopment() || _environment.IsTest() ? ex.Message : "Unexpected error",
            InternalCode = StatusCodes.Status500InternalServerError,
            StatusCode = StatusCodes.Status500InternalServerError
        };
    }

    protected virtual ErrorDetails OnUserDefinedError(HttpContext _, BaseException ex)
    {
        return new ErrorDetails
        {
            Message = ex.PublicMessage,
            InternalCode = ex.InternalCode,
            StatusCode = ex.StatusCode
        };
    }

    protected virtual async Task HandleErrorAsync(HttpContext httpContext, ErrorDetails errorDetails)
    {
        httpContext.Response.StatusCode = errorDetails.StatusCode;
        var response = new { errorDetails.Message, Code = errorDetails.InternalCode };
        await httpContext.Response.WriteAsync(JsonConvert.SerializeObject(response));
    }
}
