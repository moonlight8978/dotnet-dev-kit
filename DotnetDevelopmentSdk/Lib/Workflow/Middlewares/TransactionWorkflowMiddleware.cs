// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

using System.Data;
using System.Data.Common;
using Microsoft.EntityFrameworkCore;

namespace DotnetDevelopmentSdk.Lib.Workflow.Middlewares;

public class TransactionWorkflowMiddleware<T> : WorkflowMiddleware where T : DbContext
{
    private readonly T _dbContext;
    private DbTransaction? _transaction;

    public TransactionWorkflowMiddleware(T dbContext)
    {
        _dbContext = dbContext;
    }

    public override async Task InitializeAsync(IWorkflowContext workflowContext)
    {
        await base.InitializeAsync(workflowContext);

        var dbConnection = _dbContext.Database.GetDbConnection();

        if (dbConnection.State != ConnectionState.Open)
        {
            await dbConnection.OpenAsync();
        }

        _transaction = await dbConnection.BeginTransactionAsync();
        await _dbContext.Database.UseTransactionAsync(_transaction);
    }

    public override async Task FinalizeAsync(IWorkflowContext workflowContext)
    {
        await base.FinalizeAsync(workflowContext);

        if (_transaction == null)
        {
            return;
        }

        if (workflowContext.IsSuccess())
        {
            await _transaction.CommitAsync();
        }
        else
        {
            await _transaction.RollbackAsync();
        }
    }
}
