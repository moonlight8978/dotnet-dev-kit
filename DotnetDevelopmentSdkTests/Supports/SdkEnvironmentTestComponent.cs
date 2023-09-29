// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

using DotnetTestSdk.Lib.Components;
using DummyApp.Features.WeatherForecast;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace DotnetDevelopmentSdkTests.Supports;

public class SdkEnvironmentTestComponent : EnvironmentTestComponent<Program>
{
    public SdkEnvironmentTestComponent(ITestComponentManager componentManager) : base(componentManager)
    {
    }

    protected override void OnConfigureWebHostBuilder(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.Remove(services.Single(d => d.ServiceType == typeof(IWeatherForecastProvider)));
            services.AddScoped<IWeatherForecastProvider, RemoteWeatherForecastProvider>();
        });
    }
}
