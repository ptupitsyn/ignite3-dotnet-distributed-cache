using System.Text.Json;
using Apache.Extensions.Caching.Ignite;
using Apache.Ignite;
using Microsoft.Extensions.Caching.Distributed;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddIgniteClientGroup(new IgniteClientGroupConfiguration
    {
        ClientConfiguration = new IgniteClientConfiguration("localhost")
    })
    .AddIgniteDistributedCache(options => options.TableName = "ASPNET_DISTRIBUTED_CACHE");

var app = builder.Build();

app.UseHttpsRedirection();

app.MapGet("/weatherforecast",  async (IDistributedCache cache) =>
    {
        const string cacheKey = "weather_forecast";

        byte[]? cachedData = await cache.GetAsync(cacheKey);
        if (cachedData != null)
        {
            return JsonSerializer.Deserialize<IList<WeatherForecast>>(cachedData);
        }

        IList<WeatherForecast> forecast = FetchForecast();

        cachedData = JsonSerializer.SerializeToUtf8Bytes(forecast);
        await cache.SetAsync(cacheKey, cachedData, CancellationToken.None);

        return forecast;
    })
    .WithName("GetWeatherForecast");

app.Run();

IList<WeatherForecast> FetchForecast()
{
    var summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };
    return Enumerable.Range(1, 5).Select(index =>
            new WeatherForecast
            (
                DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                Random.Shared.Next(-20, 55),
                summaries[Random.Shared.Next(summaries.Length)]
            ))
        .ToArray();
}

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary);
