// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

using DotnetDevelopmentSdk.Lib.Database;
using DummyApp.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace DummyApp.Database;

public class AppDbContext : BasePostgreSqlDbContext<AppDbContext, DatabaseConfiguration>
{
    public AppDbContext(DbContextOptions<AppDbContext> dbContextOptions,
        IOptions<DatabaseConfiguration> databaseConfiguration) : base(dbContextOptions, databaseConfiguration)
    {
    }

    public DbSet<WeatherForecastModel> WeatherForecast { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        modelBuilder.Entity<WeatherForecastModel>().HasData(new List<WeatherForecastModel>()
        {
            new() { Id = 1, TemperatureC = 31, Summary = "Freezing", Date = new DateTime(2023, 10, 1) },
            new() { Id = 2, TemperatureC = 32, Summary = "Bracing", Date = new DateTime(2023, 10, 2) },
            new() { Id = 3, TemperatureC = 33, Summary = "Cool", Date = new DateTime(2023, 10, 3) }
        });
    }
}
