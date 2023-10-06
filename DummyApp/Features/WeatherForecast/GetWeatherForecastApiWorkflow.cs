// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

using DotnetDevelopmentSdk.Lib.Pagination;
using DotnetDevelopmentSdk.Lib.Validators;
using DotnetDevelopmentSdk.Lib.Workflow.API;
using DotnetDevelopmentSdk.Lib.Workflow.Middlewares;
using DummyApp.Database;
using DummyApp.Features.Pagination;
using DummyApp.Models;
using FluentValidation;

namespace DummyApp.Features.WeatherForecast;

public class GetWeatherForecastApiWorkflow : ApiWorkflow<GetWeatherForecastHttpRequestData,
    GetWeatherForecastHttpResponseData, GetWeatherForecastErrorCode, GetWeatherForecastApiWorkflowContext>
{
    private readonly AppDbContext _appDbContext;

    public GetWeatherForecastApiWorkflow(WorkflowMiddlewareManager workflowMiddlewareManager, AppDbContext appDbContext)
        : base(workflowMiddlewareManager)
    {
        _appDbContext = appDbContext;

        workflowMiddlewareManager
            .Use<TransactionWorkflowMiddleware<AppDbContext>>()
            .Use<DatabasePersistenceWorkflowMiddleware<AppDbContext>>()
            .Use<RequestValidationWorkflowMiddleware<GetWeatherForecastValidator, GetWeatherForecastApiWorkflowContext,
                GetWeatherForecastErrorCode>>()
            .Use<PaginationWorkflowMiddleware<WeatherForecastModel>>();
    }

    protected override async Task OnPrepareAsync()
    {
        await base.OnPrepareAsync();

        WorkflowContext.PaginationRequestData = WorkflowContext.RequestData.Pagination;
    }

    protected override async Task OnProcessAsync()
    {
        _appDbContext.Add(new WeatherForecastModel { Date = DateTime.Today });
        WorkflowContext.Query = _appDbContext.WeatherForecast
            .OrderByDescending(w => w.Date)
            .ThenByDescending(w => w.Id);
    }

    protected override void OnSuccess()
    {
        WorkflowContext.ResponseData = new GetWeatherForecastHttpResponseData()
        {
            WeatherForecasts = WorkflowContext.Result,
            Pagination = (WorkflowContext.PaginationResponseData as PaginationResponseData)!
        };
    }
}

public class GetWeatherForecastApiWorkflowContext : WorkflowContext, IPaginationWorkflowContext<WeatherForecastModel>,
    IApiWorkflowContext<GetWeatherForecastHttpRequestData, GetWeatherForecastHttpResponseData>
{
    public IPaginationRequestData PaginationRequestData { get; set; } = new PaginationRequestData();
    public IPaginationResponseData PaginationResponseData { get; set; } = new PaginationResponseData();
    public IQueryable<WeatherForecastModel>? Query { get; set; }
    public IEnumerable<WeatherForecastModel>? List { get; set; }
    public List<WeatherForecastModel> Result { get; set; } = new();
    public GetWeatherForecastHttpRequestData RequestData { get; set; } = null!;
    public GetWeatherForecastHttpResponseData? ResponseData { get; set; }
}

public class GetWeatherForecastValidator : Validator<GetWeatherForecastApiWorkflowContext, GetWeatherForecastErrorCode>
{
    public GetWeatherForecastValidator()
    {
        RuleFor(ctx => ctx).Must((ctx) =>
        {
            ctx.RequestData.Pagination.From = 0;
            return true;
        }).DependentRules(() =>
        {
            RuleFor(ctx => ctx.RequestData.Pagination).SetValidator(new PaginationValidator());
            RuleFor(ctx => ctx.RequestData.CreateNewRecord).Must(v => v)
                .WithErrorCode(GetWeatherForecastErrorCode.MustCreateNewRecord);
        });
    }
}
