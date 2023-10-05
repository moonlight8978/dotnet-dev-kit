// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

using DotnetDevelopmentSdk.Lib.Pagination;
using DotnetDevelopmentSdk.Lib.Validators;
using DotnetDevelopmentSdk.Lib.Workflow;
using DotnetDevelopmentSdk.Lib.Workflow.Middlewares;
using DummyApp.Features.Pagination;
using FluentValidation;

namespace DummyApp.Features.WeatherForecast;

public class GetWeatherForecastApiWorkflow : ApiWorkflow<GetWeatherForecastHttpRequestData,
    GetWeatherForecastHttpResponseData, GetWeatherForecastErrorCode, GetWeatherForecastApiWorkflowContext>
{
    public GetWeatherForecastApiWorkflow(WorkflowMiddlewareManager workflowMiddlewareManager) : base(
        workflowMiddlewareManager)
    {
        workflowMiddlewareManager
            .Use<RequestValidationWorkflowMiddleware<GetWeatherForecastValidator, GetWeatherForecastApiWorkflowContext,
                GetWeatherForecastErrorCode>>();
    }

    protected override Task<ApiActionServiceResult> OnProcessAsync(GetWeatherForecastHttpRequestData requestData)
    {
        throw new NotImplementedException();
    }
}

public class GetWeatherForecastApiWorkflowContext : WorkflowContext, IPaginationWorkflowContext,
    IApiWorkflowContext<GetWeatherForecastHttpRequestData, GetWeatherForecastHttpResponseData>
{
    public IPaginationRequestData PaginationRequestData { get; set; } = new PaginationRequestData();
    public IPaginationResponseData PaginationResponseData { get; set; } = new PaginationResponseData();
    public GetWeatherForecastHttpRequestData RequestData { get; set; } = null!;
    public GetWeatherForecastHttpResponseData? ResponseData { get; set; }
}

public class GetWeatherForecastValidator : Validator<GetWeatherForecastApiWorkflowContext, GetWeatherForecastErrorCode>
{
    public GetWeatherForecastValidator()
    {
        RuleFor(ctx => ctx).Must((ctx) =>
        {
            ctx.RequestData.Offset += 1;
            return true;
        }).DependentRules(() =>
        {
            RuleFor(context => context.RequestData.Offset).GreaterThanOrEqualTo(2);
        });
    }
}
