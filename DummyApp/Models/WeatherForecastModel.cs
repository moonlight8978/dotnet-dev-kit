// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DummyApp.Models;

[Table("weather_forecasts")]
[Index(nameof(Date), IsUnique = true)]
public class WeatherForecastModel
{
    [Key]
    [Column("id")]
    public int Id { get; set; }
    
    [Column("date")]
    public DateTime Date { get; set; }

    [Column("temperature_c")]
    public int TemperatureC { get; set; }

    [NotMapped]
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

    [Column("summary")]
    public string? Summary { get; set; }
}
