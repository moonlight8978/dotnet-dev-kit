// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

using DotnetDevelopmentSdk.Lib.Pagination;

namespace DotnetDevelopmentSdk.Lib.Workflow.Middlewares;

public class PaginationWorkflowMiddleware : WorkflowMiddleware
{
    private readonly PaginationService _paginationService;

    public PaginationWorkflowMiddleware(PaginationService paginationService)
    {
        _paginationService = paginationService;
    }

    public override async Task InitializeAsync(IWorkflowContext workflowContext)
    {
        await base.InitializeAsync(workflowContext);

        var context = (workflowContext as IPaginationWorkflowContext)!;
        _paginationService.SetPaginationInfoFromRequest(context.PaginationRequestData);
    }

    public override async Task FinalizeAsync(IWorkflowContext workflowContext)
    {
        await base.FinalizeAsync(workflowContext);

        var context = (workflowContext as IPaginationWorkflowContext)!;
        _paginationService.SetResponseData(context.PaginationResponseData);
    }
}
