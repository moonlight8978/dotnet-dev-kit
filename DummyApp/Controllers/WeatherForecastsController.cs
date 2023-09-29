// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

using DotnetDevelopmentSdk.Lib.Api;
using DummyApp.Features.WeatherForecast;
using Microsoft.AspNetCore.Mvc;

namespace DummyApp.Controllers;

[ApiController]
[Route("api")]
public class WeatherForecastsController : BaseApiController,
    IControllerActionHandler<GetWeatherForecastHttpRequestData, GetWeatherForecastHttpResponseData>
{
    private readonly GetWeatherForecastApiActionService _getWeatherForecastApiActionService;

    public WeatherForecastsController(GetWeatherForecastApiActionService getWeatherForecastApiActionService)
    {
        _getWeatherForecastApiActionService = getWeatherForecastApiActionService;
    }

    [HttpPost(GetWeatherForecastHttpRequestData.Endpoint)]
    public async Task<ActionResult<HttpResponseData<GetWeatherForecastHttpResponseData>>> Perform(
        HttpRequestData<GetWeatherForecastHttpRequestData> httpRequestData)
    {
        return MakeResponse<HttpResponseData<GetWeatherForecastHttpResponseData>>(
            await _getWeatherForecastApiActionService.PerformAsync(httpRequestData));
    }
}
