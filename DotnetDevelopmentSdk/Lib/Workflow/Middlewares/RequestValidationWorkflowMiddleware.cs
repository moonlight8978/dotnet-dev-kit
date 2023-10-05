// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

using DotnetDevelopmentSdk.Lib.Validators;
using FluentValidation;

namespace DotnetDevelopmentSdk.Lib.Workflow.Middlewares;

public class RequestValidationWorkflowMiddleware<TValidator, TWorkflowContext, TErrorCode> : WorkflowMiddleware
    where TWorkflowContext : class, IWorkflowContext
    where TValidator : Validator<TWorkflowContext>, IEnumErrorCodeValidator<TErrorCode>
    where TErrorCode : Enum
{
    private readonly ValidationService _validationService;

    public RequestValidationWorkflowMiddleware(ValidationService validationService)
    {
        _validationService = validationService;
    }

    public override async Task InitializeAsync(IWorkflowContext workflowContext)
    {
        await base.InitializeAsync(workflowContext);

        var context = (workflowContext as TWorkflowContext)!;

        var result = await _validationService.PerformAsync<TValidator, TWorkflowContext>(context);

        if (result.Contains<TErrorCode>(out var errorCode))
        {
            context.ResultCode = (int)(object)errorCode!;
            return;
        }

        if (result.IsInvalid)
        {
            context.ResultCode = -1;
        }
    }
}
