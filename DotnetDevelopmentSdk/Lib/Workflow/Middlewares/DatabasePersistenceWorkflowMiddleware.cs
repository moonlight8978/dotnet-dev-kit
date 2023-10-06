// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

using DotnetDevelopmentSdk.Lib.Workflow.API;
using Microsoft.EntityFrameworkCore;

namespace DotnetDevelopmentSdk.Lib.Workflow.Middlewares;

public class DatabasePersistenceWorkflowMiddleware<TDbContext> : WorkflowMiddleware where TDbContext : DbContext
{
    private readonly TDbContext _dbContext;

    public DatabasePersistenceWorkflowMiddleware(TDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public override async Task FinalizeAsync(IWorkflowContext workflowContext)
    {
        await base.FinalizeAsync(workflowContext);
        
        if (workflowContext.IsSuccess())
        {
            await _dbContext.SaveChangesAsync();
        }
    }
}
