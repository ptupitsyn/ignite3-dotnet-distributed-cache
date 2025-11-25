var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

app.UseHttpsRedirection();

app.MapGet("/weatherforecast", () =>
    {
        // TODO: Cache me.
        return FetchForecast();
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
