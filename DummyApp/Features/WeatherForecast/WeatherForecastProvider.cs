// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

using System.ComponentModel.DataAnnotations;
using DotnetDevelopmentSdk.Lib.Configurations;
using DotnetDevelopmentSdk.Lib.Services.HttpClient;
using DotnetDevelopmentSdk.Lib.Utils;
using DummyApp.Models;
using Microsoft.Extensions.Options;
using RestSharp;

namespace DummyApp.Features.WeatherForecast;

public interface IWeatherForecastProvider
{
    public Task<List<WeatherForecastModel>> GetListAsync();
}

[BindingType(typeof(IWeatherForecastProvider))]
public class LocalWeatherForecastProvider : IWeatherForecastProvider, ITypeDirectedScopeBindedService
{
    private static readonly string[] s_summaries =
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    public Task<List<WeatherForecastModel>> GetListAsync()
    {
        return Task.FromResult(Enumerable.Range(1, 5).Select(index => new WeatherForecastModel
        {
            Date = DateTime.Now.AddDays(index),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = s_summaries[Random.Shared.Next(s_summaries.Length)]
        }).ToList());
    }
}

public class RemoteWeatherForecastProvider : IWeatherForecastProvider
{
    private readonly AccuWeatherHttpClient _accuWeatherHttpClient;

    public RemoteWeatherForecastProvider(AccuWeatherHttpClient accuWeatherHttpClient)
    {
        _accuWeatherHttpClient = accuWeatherHttpClient;
    }

    public async Task<List<WeatherForecastModel>> GetListAsync()
    {
        return await _accuWeatherHttpClient.GetForecast();
    }
}

public class AccuWeatherHttpClient : BaseHttpClient, ITypeDirectedScopeBindedService
{
    private static readonly string[] s_summaries =
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    public AccuWeatherHttpClient(IOptions<AccuWeatherConfiguration> accuWeatherConfiguration) : base(
        accuWeatherConfiguration.Value.BaseUrl)
    {
    }

    public async Task<List<WeatherForecastModel>> GetForecast()
    {
        await SendRequest<object>("v2/mechas/1", Method.Get);

        return Enumerable.Range(1, 5).Select(index => new WeatherForecastModel
        {
            Date = DateTime.Now.AddDays(index),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = s_summaries[Random.Shared.Next(s_summaries.Length)]
        }).ToList();
    }
}

[CustomConfiguration("AccuWeather")]
public class AccuWeatherConfiguration : ICustomConfiguration
{
    [Required] public string BaseUrl { get; set; } = null!;
}
