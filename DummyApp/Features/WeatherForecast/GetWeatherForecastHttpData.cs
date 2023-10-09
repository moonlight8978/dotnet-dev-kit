// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

using DummyApp.Features.Pagination;
using DummyApp.Models;

namespace DummyApp.Features.WeatherForecast;

public class GetWeatherForecastHttpRequestData
{
    public const string Endpoint = "weathers/get";

    public PaginationRequestData Pagination { get; set; } = null!;
    public bool CreateNewRecord { get; set; }
}

public class GetWeatherForecastHttpResponseData
{
    public List<WeatherForecastModel> WeatherForecasts { get; set; } = new();
    public PaginationResponseData Pagination { get; set; }
}

public enum GetWeatherForecastErrorCode
{
    MustCreateNewRecord = 1
}
