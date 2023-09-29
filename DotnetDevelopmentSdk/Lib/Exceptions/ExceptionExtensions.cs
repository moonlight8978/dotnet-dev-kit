// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DotnetDevelopmentSdk.Lib.Exceptions;

public static class ExceptionExtensions
{
    public static void AddExceptionHandler<T>(this IServiceCollection services)
        where T : BaseExceptionHandlingMiddleware
    {
        services.AddTransient<T>();
    }

    public static void UseExceptionHandler<T>(this WebApplication app)
        where T : BaseExceptionHandlingMiddleware
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseMiddleware<T>();
    }
}
