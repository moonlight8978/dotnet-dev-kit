// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

using DotnetDevelopmentSdk.Lib.Api;
using DotnetDevelopmentSdk.Lib.Workflow.API;
using DummyApp.Features.WeatherForecast;
using Microsoft.AspNetCore.Mvc;

namespace DummyApp.Controllers;

[ApiController]
[Route("api")]
public class WeatherForecastsController : APIController,
    IControllerActionHandler<GetWeatherForecastHttpRequestData, GetWeatherForecastHttpResponseData>
{
    private readonly GetWeatherForecastApiWorkflow _getWeatherForecastApiWorkflow;

    [HttpPost(GetWeatherForecastHttpRequestData.Endpoint)]
    public async Task<ActionResult<HttpResponseData<GetWeatherForecastHttpResponseData>>> Perform(
        HttpRequestData<GetWeatherForecastHttpRequestData> httpRequestData)
    {
        return Ok(await _getWeatherForecastApiWorkflow.PerformAsync(httpRequestData.Data));
    }

    public WeatherForecastsController(IErrorCodeProvider errorCodeProvider,
        GetWeatherForecastApiWorkflow getWeatherForecastApiWorkflow) : base(errorCodeProvider)
    {
        _getWeatherForecastApiWorkflow = getWeatherForecastApiWorkflow;
    }
}
