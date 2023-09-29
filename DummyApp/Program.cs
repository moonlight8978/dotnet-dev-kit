// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

using DotnetDevelopmentSdk.Lib.Configurations;
using DotnetDevelopmentSdk.Lib.Logging;
using DotnetDevelopmentSdk.Lib.Utils;
using DummyApp.Features.WeatherForecast;

var builder = WebApplication.CreateBuilder(args);

builder.AddConfiguration<AccuWeatherConfiguration>();
builder.AddSerilog();

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.BindTypeDirectedScopedServices();
builder.Services.BindTypeDirectedSingletonServices();
builder.Services.BindTypeDirectedTransientServices();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program
{
}
