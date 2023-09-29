// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

using Datadog.Trace;
using Datadog.Trace.Configuration;
using DotnetDevelopmentSdk.Lib.Configurations;
using DotnetDevelopmentSdk.Lib.Environment;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using Serilog.Exceptions;

namespace DotnetDevelopmentSdk.Lib.Logging;

public static class LoggingExtensions
{
    public static WebApplicationBuilder AddSerilog(this WebApplicationBuilder builder)
    {
        builder.Host.UseSerilog(LoggingBoostrapper.ConfigureLogger);

        return builder;
    }

    public static void AddUnhandledExceptionHandler(this AppDomain appDomain)
    {
        appDomain.UnhandledException += (_, eventArgs) =>
        {
            var e = (Exception)eventArgs.ExceptionObject;
            Log.Error(e, $"UnhandledException. Runtime terminating: {eventArgs.IsTerminating}");
        };

        TaskScheduler.UnobservedTaskException += (_, eventArgs) =>
        {
            var e = (Exception)eventArgs.Exception;
            Log.Error(e, $"UnobservedTaskException exception. Is Observed : {eventArgs.Observed}");
        };
    }

    public static LoggerConfiguration AddBasicLog(this LoggerConfiguration loggerConfiguration)
    {
        return loggerConfiguration.MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            .Enrich.FromLogContext()
            .WriteTo.Console();
    }

    public static LoggerConfiguration AddAdvancedLog(this LoggerConfiguration loggerConfiguration)
    {
        return loggerConfiguration.Enrich.FromLogContext()
            .Enrich.WithExceptionDetails()
            .Enrich.WithMachineName()
            .WriteTo.Console(
                outputTemplate:
                "[{Timestamp:u} {Level}] {FileName}:{LineNumber} - {RequestId} - {Message}{NewLine}{Exception}");
    }

    public static void AddRequestLog(this IServiceCollection services, IWebHostEnvironment webHostEnvironment)
    {
        if (webHostEnvironment.IsDevelopment() || webHostEnvironment.IsTest())
        {
            services.AddHttpLogging(options =>
            {
                options.LoggingFields = HttpLoggingFields.RequestBody;
            });
        }
    }

    public static void UseRequestLog(this WebApplication app)
    {
        app.UseSerilogRequestLogging();
        if (app.Environment.IsDevelopment() || app.Environment.IsTest())
        {
            app.UseHttpLogging();
        }
    }

    public static WebApplicationBuilder AddTracer(this WebApplicationBuilder builder)
    {
        var tracerSettings = TracerSettings.FromDefaultSources();

        var applicationLoggerConfiguration = builder.Configuration.GetCustom<ApplicationLoggerConfiguration>();

        tracerSettings.ServiceName = applicationLoggerConfiguration.Tag;
        tracerSettings.Environment = builder.Environment.EnvironmentName.ToLower();

        Tracer.Configure(tracerSettings);

        return builder;
    }

    public static void Trace(this Tracer tracer, Action<ISpan> action)
    {
        tracer.ActiveScope.Trace(action);
    }

    public static void Trace(this IScope scope, Action<ISpan> action)
    {
        if (scope == null)
        {
            return;
        }

        try
        {
            action.Invoke(scope.Span);
        }
        catch (Exception)
        {
            //
        }
    }

    public static void SetTag<TEnum>(this ISpan span, string key, TEnum value) where TEnum : Enum
    {
        span.SetTag(key, value.ToString());
    }
}
