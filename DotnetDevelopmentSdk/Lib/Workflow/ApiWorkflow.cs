// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

using DotnetDevelopmentSdk.Lib.Api;
using DotnetDevelopmentSdk.Lib.Workflow.Middlewares;
using Serilog;
using IPaginationRequestData = DotnetDevelopmentSdk.Lib.Pagination.IPaginationRequestData;
using IPaginationResponseData = DotnetDevelopmentSdk.Lib.Pagination.IPaginationResponseData;

namespace DotnetDevelopmentSdk.Lib.Workflow;

public interface IWorkflowContext
{
    public int ResultCode { get; set; }
    public bool IsSuccess();
    public bool IsFailure();
}

public class WorkflowContext : IWorkflowContext
{
    public int ResultCode { get; set; }

    public bool IsSuccess() => ResultCode == 0;

    public bool IsFailure() => !IsSuccess();
}

public interface IApiWorkflowContext<TRequestData, TResponseData> : IWorkflowContext
{
    public TRequestData RequestData { get; set; }
    public TResponseData? ResponseData { get; set; }
}

public interface IPaginationWorkflowContext : IWorkflowContext
{
    public IPaginationRequestData PaginationRequestData { get; set; }
    public IPaginationResponseData PaginationResponseData { get; set; }
}

public abstract class ApiWorkflow<TRequestData, TResponseData, TErrorCode, TWorkflowContext>
    where TErrorCode : Enum
    where TResponseData : class, new()
    where TRequestData : class, new()
    where TWorkflowContext : class, IApiWorkflowContext<TRequestData, TResponseData>, new()
{
    
    
    protected readonly ILogger Logger;
    protected readonly TWorkflowContext WorkflowContext = new();
    private readonly WorkflowMiddlewareManager _workflowMiddlewareManager;
    protected readonly ApiActionServiceResultFactory<TResponseData, TErrorCode> ResultFactory = new();

    protected ApiWorkflow(WorkflowMiddlewareManager workflowMiddlewareManager)
    {
        _workflowMiddlewareManager = workflowMiddlewareManager;
        Logger = Log.ForContext(GetType());
    }

    public async Task<ApiActionServiceResult> PerformAsync(TRequestData requestData)
    {
        try
        {
            WorkflowContext.RequestData = requestData;
            await OnPrepareAsync(requestData);
            await _workflowMiddlewareManager.InitializeAsync(WorkflowContext);
            var result = await OnProcessAsync(requestData);
            WorkflowContext.ResultCode = result.Code;
            WorkflowContext.ResponseData = result.ResponseData as TResponseData;
            return result;
        }
        catch (Exception)
        {
            WorkflowContext.ResultCode = -1;
            throw;
        }
        finally
        {
            await _workflowMiddlewareManager.FinalizeAsync(WorkflowContext);
        }
    }

    protected virtual Task OnPrepareAsync(TRequestData requestData)
    {
        return Task.CompletedTask;
    }

    protected abstract Task<ApiActionServiceResult> OnProcessAsync(TRequestData requestData);
}
