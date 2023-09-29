// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

using DotnetDevelopmentSdk.Lib.Environment;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace DotnetDevelopmentSdk.Lib.Configurations;

public static class ConfigurationExtensions
{
    public static void AddConfigurationFiles(this IConfigurationBuilder configurationBuilder,
        IHostEnvironment hostEnvironment, params ConfigurationFileInfo[] configurationFileInfos)
    {
        foreach (var configurationFileInfo in configurationFileInfos)
        {
            configurationBuilder.AddJsonFile(configurationFileInfo.Location(), true)
                .AddJsonFile(configurationFileInfo.Location(hostEnvironment.EnvironmentName));

            if (hostEnvironment.IsTest() || hostEnvironment.IsDevelopment())
            {
                configurationBuilder.AddJsonFile(
                    configurationFileInfo.Location($"{hostEnvironment.EnvironmentName}.Local"), true);
            }
        }

        configurationBuilder.AddEnvironmentVariables();
    }

    public static IServiceCollection AddCustomConfiguration<T>(this IServiceCollection services,
        IConfiguration configuration)
        where T : class, ICustomConfiguration
    {
        var config =
            (CustomConfigurationAttribute?)Attribute.GetCustomAttribute(typeof(T),
                typeof(CustomConfigurationAttribute));

        if (config?.Key == null)
        {
            throw new ArgumentException($"Custom configuration key must be defined. {typeof(T).FullName}");
        }

        services.AddOptions<T>().Bind(configuration.GetSection(config.Key)).ValidateDataAnnotations();

        if (config.ValidatorType != null)
        {
            services.AddSingleton(typeof(IValidateOptions<T>), config.ValidatorType);
        }

        return services;
    }

    public static T GetCustom<T>(this IConfiguration configuration) where T : ICustomConfiguration
    {
        var customConfigurationAttribute =
            (CustomConfigurationAttribute?)Attribute.GetCustomAttribute(typeof(T),
                typeof(CustomConfigurationAttribute));
        if (customConfigurationAttribute == null)
        {
            throw new NullReferenceException($"Please define `CustomConfigurationAttribute` on {typeof(T).FullName}");
        }

        var customConfiguration = configuration.GetSection(customConfigurationAttribute.Key).Get<T>();
        return customConfiguration;
    }

    public static WebApplicationBuilder AddConfiguration<T>(this WebApplicationBuilder builder)
        where T : class, ICustomConfiguration
    {
        builder.Services.AddCustomConfiguration<T>(builder.Configuration);
        return builder;
    }

    public static WebApplicationBuilder AddCustomConfigurations(this WebApplicationBuilder builder,
        string projectFolder)
    {
        builder.Configuration.AddConfigurationFiles(builder.Environment, projectFolder);

        return builder;
    }

    private static void AddConfigurationFiles(this IConfigurationBuilder configurationBuilder,
        IHostEnvironment hostEnvironment, string projectFolder)
    {
        configurationBuilder.AddConfigurationFiles(hostEnvironment,
            new ConfigurationFileInfo("app",
                Path.Combine(hostEnvironment.ContentRootPath, "..", projectFolder)));
    }
}
