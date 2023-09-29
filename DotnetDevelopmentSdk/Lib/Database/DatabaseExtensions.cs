// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace DotnetDevelopmentSdk.Lib.Database;

public static class DatabaseExtensions
{
    public static WebApplication MigrateDatabase<T>(this WebApplication app) where T : DbContext
    {
        using var scope = app.Services.CreateScope();

        var dbContext = scope.ServiceProvider.GetRequiredService<T>();
        try
        {
            Log.Logger.Information("Migrating database...");
            dbContext.Database.Migrate();
            Log.Logger.Information("Database migrated successfully");
        }
        catch (Exception ex)
        {
            Log.Logger.Fatal(ex, "Cannot migrate game database");
            throw;
        }

        return app;
    }

    public static void AddDatabase<T>(this IServiceCollection services) where T : DbContext
    {
        services.AddDbContext<T>();
        services.AddDatabaseDeveloperPageExceptionFilter();
    }

    public static void AddDatabase<T1, T2>(this IServiceCollection services) where T1 : DbContext where T2 : DbContext
    {
        services.AddDbContext<T1>();
        services.AddDbContext<T2>();
        services.AddDatabaseDeveloperPageExceptionFilter();
    }

    public static void AddDatabase<T1, T2, T3>(this IServiceCollection services)
        where T1 : DbContext where T2 : DbContext where T3 : DbContext
    {
        services.AddDbContext<T1>();
        services.AddDbContext<T2>();
        services.AddDbContext<T3>();
        services.AddDatabaseDeveloperPageExceptionFilter();
    }
}
