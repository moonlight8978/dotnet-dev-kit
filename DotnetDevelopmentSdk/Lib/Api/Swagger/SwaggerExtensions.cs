// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace DotnetDevelopmentSdk.Lib.Api.Swagger;

public static class SwaggerExtensions
{
    public static void AddApiDocs(this IServiceCollection services, Action<SwaggerGenOptions>? configure = null,
        params string[] projectNames)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.EnableAnnotations();

            // TODO: Automate
            foreach (var projectName in projectNames)
            {
                options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, $"{projectName}.xml"));
            }

            options.CustomSchemaIds(type => type.FullName);

            options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "DotnetDevelopmentSdk.xml"));

            configure?.Invoke(options);
        });
        services.AddSwaggerGenNewtonsoftSupport();
    }

    public static void UseApiDocs(this WebApplication app,
        string? docsTemplate = null,
        Action<SwaggerUIOptions>? configureUserInterface = null)
    {
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.IndexStream = () =>
            {
                if (docsTemplate == null)
                {
                    return Assembly.GetExecutingAssembly()
                        .GetManifestResourceStream("DotnetDevelopmentSdk.Lib.Api.Swagger.Templates.index.html");
                }

                return Assembly.GetEntryAssembly()!.GetManifestResourceStream(docsTemplate);
            };

            configureUserInterface?.Invoke(options);
        });
    }
}
