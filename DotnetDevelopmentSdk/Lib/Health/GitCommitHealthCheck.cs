// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace DotnetDevelopmentSdk.Lib.Health;

public class GitCommitHealthCheck : IHealthCheck
{
    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context,
        CancellationToken cancellationToken = new())
    {
        var commitIds = ParseCommitIds();
        return Task.FromResult(HealthCheckResult.Healthy("git commit id",
            new Dictionary<string, object> { { "commitIds", commitIds } }));
    }

    protected virtual string[] ParseCommitIds()
    {
        return new[] { System.Environment.GetEnvironmentVariable("REPOSITORY_COMMIT_ID") ?? "" };
    }
}

public static class GitCommitHealthCheckExtensions
{
    public static IHealthChecksBuilder AddGitCommitHealthCheck<THealthCheck>(this IHealthChecksBuilder healthChecksBuilder)
        where THealthCheck : GitCommitHealthCheck
    {
        healthChecksBuilder.AddCheck<THealthCheck>("GitCommit", tags: new[] { "git-commit" });
        return healthChecksBuilder;
    }

    public static void UseGitCommitHealthCheck(this WebApplication app)
    {
        app.MapHealthChecks("/build", new HealthCheckOptions
        {
            Predicate = healthcheck => healthcheck.Tags.Contains("git-commit"),
            ResponseWriter = async (context, report) =>
            {
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(JsonConvert.SerializeObject(new
                {
                    CommitIds = report.Entries.SelectMany(entry =>
                        (string[])entry.Value.Data["commitIds"])
                },
                    new JsonSerializerSettings
                    {
                        ContractResolver = new CamelCasePropertyNamesContractResolver(),
                        NullValueHandling = NullValueHandling.Ignore
                    }));
            }
        });
    }
}
