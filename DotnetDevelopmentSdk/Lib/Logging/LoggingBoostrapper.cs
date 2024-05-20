// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

using System.Net;
using System.Text.RegularExpressions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Serilog;
using Serilog.Exceptions;

namespace DotnetDevelopmentSdk.Lib.Logging;

public static class LoggingBoostrapper
{
    public static void Boostrap(Action action)
    {
        Log.Logger = new LoggerConfiguration()
            .AddBasicLog()
            .CreateBootstrapLogger();

        try
        {
            action.Invoke();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Host terminated unexpectedly");
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }

    public static void ConfigureLogger(HostBuilderContext context, IServiceProvider provider,
        LoggerConfiguration configuration)
    {
        var applicationLoggerConfiguration =
            provider.GetRequiredService<IOptions<ApplicationLoggerConfiguration>>().Value;

        configuration.ReadFrom.Configuration(context.Configuration)
            .Enrich.FromLogContext()
            .Enrich.WithExceptionDetails()
            .Enrich.WithMachineName()
            .WriteTo.Console(
                outputTemplate:
                "[{Timestamp:u} {Level}] {FileName}:{LineNumber} - {RequestId} - {Message}{NewLine}{Exception}")
            // .WriteTo.Console(new CompactJsonFormatter())
            .Filter.ByExcluding($"RequestPath = '{applicationLoggerConfiguration.HealthCheckPath}'")
            .Filter.ByExcluding($"RequestPath = '{applicationLoggerConfiguration.BuildInfoPath}'");

        if (new Regex(@"(test|development)", RegexOptions.IgnoreCase).IsMatch(context.HostingEnvironment
                .EnvironmentName))
        {
            configuration.WriteTo.File(Path.Join(
                    context.HostingEnvironment.ContentRootPath,
                    $".log/{context.HostingEnvironment.EnvironmentName}-{HostName}-.log"),
                outputTemplate:
                "[{Timestamp:u} {Level}] {FileName}:{LineNumber} - {RequestId} - {Message}{NewLine}{Exception}",
                rollingInterval: RollingInterval.Day
            );
        }
    }

    private static string HostName => Dns.GetHostName().ToLower().Replace(".", "-");
}
