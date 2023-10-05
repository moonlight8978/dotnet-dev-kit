// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

using DotnetDevelopmentSdk.Lib.Api;
using DotnetDevelopmentSdk.Lib.Utils;
using DotnetDevelopmentSdk.Lib.Validators;
using DotnetDevelopmentSdk.Lib.Workflow;

namespace DummyApp.Features.WeatherForecast;

public class GetWeatherForecastApiActionService : BaseApiActionServiceV2<GetWeatherForecastHttpRequestData,
    GetWeatherForecastHttpResponseData, GetWeatherForecastErrorCode>, ITypeDirectedScopeBindedService
{
    private readonly IWeatherForecastProvider _weatherForecastProvider;

    public GetWeatherForecastApiActionService(IValidatorV2<GetWeatherForecastHttpRequestData> requestValidator,
        IWeatherForecastProvider weatherForecastProvider) : base(
        requestValidator)
    {
        _weatherForecastProvider = weatherForecastProvider;
    }

    protected override async Task<ApiActionServiceResult> OnProcessAsync(
        GetWeatherForecastHttpRequestData apiRequestData)
    {
        var weatherForecast = await _weatherForecastProvider.GetListAsync();
        return ResultFactory.Success(new GetWeatherForecastHttpResponseData { WeatherForecasts = weatherForecast });
    }
}
