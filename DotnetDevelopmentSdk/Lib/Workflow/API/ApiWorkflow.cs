// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

using DotnetDevelopmentSdk.Lib.Utils;
using DotnetDevelopmentSdk.Lib.Workflow.Middlewares;
using Serilog;
using IPaginationRequestData = DotnetDevelopmentSdk.Lib.Pagination.IPaginationRequestData;
using IPaginationResponseData = DotnetDevelopmentSdk.Lib.Pagination.IPaginationResponseData;

namespace DotnetDevelopmentSdk.Lib.Workflow.API;

public interface IApiWorkflowContext<TRequestData, TResponseData> : IWorkflowContext
{
    public TRequestData RequestData { get; set; }
    public TResponseData? ResponseData { get; set; }
}

public interface IPaginationWorkflowContext<TItem> : IWorkflowContext
{
    public IPaginationRequestData PaginationRequestData { get; set; }
    public IPaginationResponseData PaginationResponseData { get; set; }
    public IQueryable<TItem>? Query { get; set; }
    public IEnumerable<TItem>? List { get; set; }
    public List<TItem> Result { get; set; }
}

public abstract class
    ApiWorkflow<TRequestData, TResponseData, TErrorCode, TWorkflowContext> : ITypeDirectedScopeBindedService
    where TErrorCode : Enum
    where TResponseData : class, new()
    where TRequestData : class, new()
    where TWorkflowContext : class, IApiWorkflowContext<TRequestData, TResponseData>, new()
{
    protected readonly ILogger Logger;
    protected readonly TWorkflowContext WorkflowContext = new();
    protected readonly WorkflowMiddlewareManager WorkflowMiddlewareManager;

    protected ApiWorkflow(WorkflowMiddlewareManager workflowMiddlewareManager)
    {
        WorkflowMiddlewareManager = workflowMiddlewareManager;
        Logger = Log.ForContext(GetType());
    }

    public async Task<IApiWorkflowContext<TRequestData, TResponseData>> PerformAsync(TRequestData requestData)
    {
        WorkflowContext.RequestData = requestData;

        if (WorkflowContext.IsFailure())
        {
            return WorkflowContext;
        }

        try
        {
            await OnPrepareAsync();
            await WorkflowMiddlewareManager.InitializeAsync(WorkflowContext);
            if (!WorkflowContext.IsFailure())
            {
                await OnProcessAsync();
            }
        }
        catch (Exception e)
        {
            WorkflowContext.ResultCode = -1;
            throw;
        }
        finally
        {
            await WorkflowMiddlewareManager.FinalizeAsync(WorkflowContext);
        }

        if (WorkflowContext.IsSuccess())
        {
            OnSuccess();
        }

        return WorkflowContext;
    }

    protected virtual void OnSuccess()
    {
    }

    protected virtual Task OnPrepareAsync()
    {
        return Task.CompletedTask;
    }

    protected abstract Task OnProcessAsync();

    protected void Failure(TErrorCode errorCode, string? message = null)
    {
        Failure(Convert.ToInt32(errorCode), message);
    }

    protected void Failure(int errorCode, string? message = null)
    {
        WorkflowContext.Failure(errorCode, message ?? errorCode.ToString());
    }

    protected void Success(TResponseData responseData, string message = "Success")
    {
        WorkflowContext.ResponseData = responseData;
        WorkflowContext.Success(message);
    }
}
