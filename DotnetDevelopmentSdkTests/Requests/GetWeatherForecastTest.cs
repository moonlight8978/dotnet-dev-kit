// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

using System.Net;
using DotnetDevelopmentSdk.Lib.Api;
using DotnetDevelopmentSdkTests.Supports;
using DotnetTestSdk.Lib.Extensions;
using DummyApp.Features.WeatherForecast;
using FluentAssertions;
using Newtonsoft.Json;
using NUnit.Framework;
using Encoding = System.Text.Encoding;
using PaginationRequestData = DummyApp.Features.Pagination.PaginationRequestData;

namespace DotnetDevelopmentSdkTests.Requests;

public class GetWeatherForecastTest : SdkIntegrationTestSuit
{
    private HttpClient _client = null!;

    protected override async Task OnPrepareAsync()
    {
        await base.OnPrepareAsync();

        _client = Environment.DefaultHttpClient;
    }

    [Test]
    public async Task GetSuccessfullyWithMockData()
    {
        var response = await _client.PostAsync("api/weathers/get",
            new StringContent(
                JsonConvert.SerializeObject(new HttpRequestData<GetWeatherForecastHttpRequestData>()
                {
                    Data = new GetWeatherForecastHttpRequestData()
                    {
                        Pagination = new PaginationRequestData()
                        {
                            From = 0,
                            To = 10
                        }, CreateNewRecord = true
                    }
                }), Encoding.UTF8, "application/json"));
        response.Should().HaveStatusCode(HttpStatusCode.OK);
        var responseData =
            await response.Content.ReadFromJsonAsync<HttpResponseData<GetWeatherForecastHttpResponseData>>();
        responseData.Data.WeatherForecasts.Should().HaveCount(5);
    }
}
