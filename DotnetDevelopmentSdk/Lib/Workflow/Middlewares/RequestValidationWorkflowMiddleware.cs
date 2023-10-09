// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

using DotnetDevelopmentSdk.Lib.Validators;
using DotnetDevelopmentSdk.Lib.Workflow.API;

namespace DotnetDevelopmentSdk.Lib.Workflow.Middlewares;

public class RequestValidationWorkflowMiddleware<TValidator, TWorkflowContext, TErrorCode> : WorkflowMiddleware
    where TWorkflowContext : class, IWorkflowContext
    where TValidator : Validator<TWorkflowContext>
    where TErrorCode : Enum
{
    private readonly ValidationService _validationService;
    private readonly IErrorCodeProvider _errorCodeProvider;

    public RequestValidationWorkflowMiddleware(ValidationService validationService,
        IErrorCodeProvider errorCodeProvider)
    {
        _validationService = validationService;
        _errorCodeProvider = errorCodeProvider;
    }

    public override async Task InitializeAsync(IWorkflowContext workflowContext)
    {
        await base.InitializeAsync(workflowContext);

        var context = (workflowContext as TWorkflowContext)!;

        var result = await _validationService.PerformAsync<TValidator, TWorkflowContext>(context);

        if (result.Contains<TErrorCode>(out var errorCode))
        {
            context.ResultCode = (int)(object)errorCode!;
            context.ResultMessages = result.FailureReasons;
            return;
        }

        if (result.IsInvalid)
        {
            context.ResultCode = _errorCodeProvider.ValidationError;
            context.ResultMessages = new List<string> { "Request data is invalid" };
        }
    }
}
