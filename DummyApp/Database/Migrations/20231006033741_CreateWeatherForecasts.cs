using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DummyApp.Database.Migrations
{
    /// <inheritdoc />
    public partial class CreateWeatherForecasts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "weather_forecasts",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    date = table.Column<DateTime>(type: "date", nullable: false),
                    temperature_c = table.Column<int>(type: "integer", nullable: false),
                    summary = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_weather_forecasts", x => x.id);
                });

            migrationBuilder.InsertData(
                table: "weather_forecasts",
                columns: new[] { "id", "date", "summary", "temperature_c" },
                values: new object[,]
                {
                    { 1, new DateTime(2023, 10, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Freezing", 31 },
                    { 2, new DateTime(2023, 10, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), "Bracing", 32 },
                    { 3, new DateTime(2023, 10, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), "Cool", 33 }
                });

            migrationBuilder.CreateIndex(
                name: "ix_weather_forecasts_date",
                table: "weather_forecasts",
                column: "date");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "weather_forecasts");
        }
    }
}
