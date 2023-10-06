// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

using DotnetDevelopmentSdk.Lib.Pagination;
using DotnetDevelopmentSdk.Lib.Workflow.API;

namespace DotnetDevelopmentSdk.Lib.Workflow.Middlewares;

public class PaginationWorkflowMiddleware<TItem> : WorkflowMiddleware
{
    private readonly PaginationService _paginationService;

    public PaginationWorkflowMiddleware(PaginationService paginationService)
    {
        _paginationService = paginationService;
    }

    public override async Task InitializeAsync(IWorkflowContext workflowContext)
    {
        await base.InitializeAsync(workflowContext);

        var context = (workflowContext as IPaginationWorkflowContext<TItem>)!;
        _paginationService.SetPaginationInfoFromRequest(context.PaginationRequestData);
    }

    public override async Task FinalizeAsync(IWorkflowContext workflowContext)
    {
        await base.FinalizeAsync(workflowContext);
        
        var context = (workflowContext as IPaginationWorkflowContext<TItem>)!;

        if (workflowContext.IsFailure())
        {
            return;
        }

        if (context.List != null)
        {
            context.Result = _paginationService.PerformAsync(context.List).ToList();
        }

        if (context.Query != null)
        {
            context.Result = (await _paginationService.PerformAsync(context.Query)).ToList();
        }
        
        _paginationService.SetResponseData(context.PaginationResponseData);
    }
}
