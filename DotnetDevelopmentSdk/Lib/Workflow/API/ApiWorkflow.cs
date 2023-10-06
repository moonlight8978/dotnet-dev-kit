// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

using DotnetDevelopmentSdk.Lib.Utils;
using DotnetDevelopmentSdk.Lib.Workflow.Middlewares;
using Serilog;
using IPaginationRequestData = DotnetDevelopmentSdk.Lib.Pagination.IPaginationRequestData;
using IPaginationResponseData = DotnetDevelopmentSdk.Lib.Pagination.IPaginationResponseData;

namespace DotnetDevelopmentSdk.Lib.Workflow.API;

public interface IWorkflowContext
{
    public int ResultCode { get; set; }
    public List<string> ResultMessages { get; set; }
    public bool IsSuccess();
    public bool IsFailure();
    public void Failure(int errorCode, string errorMessage);
    public void Success(string message);
}

public class WorkflowContext : IWorkflowContext
{
    public int ResultCode { get; set; }

    public List<string> ResultMessages { get; set; } = new();

    public bool IsSuccess() => ResultCode == 0;

    public bool IsFailure() => !IsSuccess();

    public void Failure(int errorCode, string errorMessage)
    {
        ResultCode = errorCode;
        if (!string.IsNullOrEmpty(errorMessage))
        {
            ResultMessages.Add(errorMessage);
        }
    }

    public void Success(string message)
    {
        ResultCode = 0;
        ResultMessages = new List<string> { message };
    }
}

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
    private readonly WorkflowMiddlewareManager _workflowMiddlewareManager;

    protected ApiWorkflow(WorkflowMiddlewareManager workflowMiddlewareManager)
    {
        _workflowMiddlewareManager = workflowMiddlewareManager;
        Logger = Log.ForContext(GetType());
    }

    public async Task<IApiWorkflowContext<TRequestData, TResponseData>> PerformAsync(TRequestData requestData)
    {
        try
        {
            WorkflowContext.RequestData = requestData;
            await OnPrepareAsync();
            await _workflowMiddlewareManager.InitializeAsync(WorkflowContext);
            if (!WorkflowContext.IsFailure())
            {
                await OnProcessAsync();
            }
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

        OnSuccess();
        
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
        WorkflowContext.Failure(Convert.ToInt32(errorCode), message ?? errorCode.ToString());
    }
}
