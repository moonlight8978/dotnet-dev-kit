// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

using System.Net;
using DotnetDevelopmentSdk.Lib.Api;
using Microsoft.AspNetCore.Mvc;

namespace DotnetDevelopmentSdk.Lib.Workflow.API;

[ApiController]
public class APIController : ControllerBase
{
    private readonly IErrorCodeProvider _errorCodeProvider;

    public APIController(IErrorCodeProvider errorCodeProvider)
    {
        _errorCodeProvider = errorCodeProvider;
    }

    public ActionResult<HttpResponseData<TResponseData>> Ok<TRequestData, TResponseData>(
        IApiWorkflowContext<TRequestData, TResponseData> workflowContext) where TResponseData : class, new()
    {
        return CreateResponse(workflowContext, HttpStatusCode.OK);
    }

    public ActionResult<HttpResponseData<TResponseData>> CreateResponse<TRequestData, TResponseData>(
        IApiWorkflowContext<TRequestData, TResponseData> workflowContext, HttpStatusCode httpStatusCode)
        where TResponseData : class, new()
    {
        if (workflowContext.IsSuccess())
        {
            return new JsonResult(workflowContext.ResponseData) { StatusCode = (int)httpStatusCode };
        }

        if (_errorCodeProvider.IsClientError(workflowContext.ResultCode))
        {
            return BadRequest(new ErrorResponse()
            {
                Code = workflowContext.ResultCode, Message = string.Join("\n", workflowContext.ResultMessages)
            });
        }

        if (workflowContext.ResultCode == _errorCodeProvider.NotFound)
        {
            return NotFound(new ErrorResponse { Code = workflowContext.ResultCode, Message = "Not found" });
        }

        return StatusCode((int)HttpStatusCode.InternalServerError);
    }
}
