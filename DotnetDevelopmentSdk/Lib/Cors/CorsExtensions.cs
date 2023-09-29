// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

using DotnetDevelopmentSdk.Lib.Configurations;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DotnetDevelopmentSdk.Lib.Cors;

public static class CorsExtensions
{
    public static void AddDefaultCorsPolicy(this IServiceCollection services, IConfiguration configuration)
    {
        var corsConfiguration = configuration.GetCustom<CorsConfiguration>();

        services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy =>
            {
                if (corsConfiguration.AllowedHosts.Length == 0)
                {
                    policy.AllowAnyOrigin();
                }
                else
                {
                    policy.WithOrigins(corsConfiguration.AllowedHosts);
                }

                if (corsConfiguration.AllowedMethods.Length == 0)
                {
                    policy.AllowAnyMethod();
                }
                else
                {
                    policy.WithMethods(corsConfiguration.AllowedMethods);
                }

                if (corsConfiguration.AllowedHeaders.Length == 0)
                {
                    policy.AllowAnyHeader();
                }
                else
                {
                    policy.WithHeaders(corsConfiguration.AllowedHeaders);
                }

                if (corsConfiguration.AllowedExposedHeaders.Length > 0)
                {
                    policy.WithExposedHeaders(corsConfiguration.AllowedExposedHeaders);
                }

                if (corsConfiguration.AllowCredentials)
                {
                    policy.AllowCredentials();
                }
            });
        });
    }

    public static void UseDefaultCorsPolicy(this WebApplication webApplication)
    {
        webApplication.UseCors();
    }
}
