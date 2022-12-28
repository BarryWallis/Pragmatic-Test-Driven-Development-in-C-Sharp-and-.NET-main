using AdamTibi.OpenWeather;

using Microsoft.AspNetCore.Mvc;

namespace Uqs.Weather.Controllers;
[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private const int ForecastDays = 5;
    private const decimal OceansideLatitude = 33.19119M;
    private const decimal OceansideLongitude = -117.35741M;
    private static readonly string[] _summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };
    private readonly ILogger<WeatherForecastController> _logger;
    private readonly IConfiguration _configuration;

    public WeatherForecastController(ILogger<WeatherForecastController> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    [HttpGet(Name = "GetWeatherForecast")]
    public IEnumerable<WeatherForecast> Get() => Enumerable.Range(1, 5).Select(index => new WeatherForecast
    {
        Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
        TemperatureC = Random.Shared.Next(-20, 55),
        Summary = _summaries[Random.Shared.Next(_summaries.Length)]
    })
        .ToArray();

    /// <summary>
    /// Get a list of random weather forecasts.
    /// </summary>
    /// <returns>A list of random weather forecasts.</returns>
    [HttpGet("GetRandomWeatherForecast")]
    public IEnumerable<WeatherForecast> GetRandom()
    {
        WeatherForecast[] weatherForecasts = new WeatherForecast[ForecastDays];
        for (int i = 0; i < weatherForecasts.Length; i++)
        {
            weatherForecasts[i] = new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now).AddDays(i + 1),
                TemperatureC = Random.Shared.Next(-0, 55)
            };
            weatherForecasts[i].Summary = MapFeelToTemperature(weatherForecasts[i].TemperatureC);
        }

        return weatherForecasts;
    }

    /// <summary>
    /// Get a lsit of actual weather forecasts.
    /// </summary>
    /// <returns>The list of weather forecasts.</returns>
    [HttpGet("GetRealWeatherForecast")]
    public async Task<IEnumerable<WeatherForecast>> GetReal()
    {
        string apiKey = _configuration["OpenWeather:Key"]
                        ?? throw new KeyNotFoundException("Configurtion for OpenWeather:Key not found.");
        HttpClient httpClient = new();
        Client openWeatherClient = new(apiKey, httpClient);
        OneCallResponse oneCallResponse
            = await openWeatherClient
                    .OneCallAsync(OceansideLatitude,
                                  OceansideLongitude,
                                  new[] { Excludes.Current, Excludes.Minutely, Excludes.Hourly, Excludes.Alerts },
                                  Units.Metric);

        WeatherForecast[] weatherForecasts = new WeatherForecast[ForecastDays];
        for (int i = 0; i < weatherForecasts.Length; i++)
        {
            weatherForecasts[i] = new WeatherForecast
            {
                Date = DateOnly.FromDateTime(oneCallResponse.Daily[i + 1].Dt),
                TemperatureC = (int)Math.Round(oneCallResponse.Daily[i + 1].Temp.Day)
            };
            weatherForecasts[i].Summary = MapFeelToTemperature(weatherForecasts[i].TemperatureC);
        }

        return weatherForecasts;
    }

    [HttpGet("ConvertCToF")]
    public double ConvertCToF(double celsius)
    {
        double farenheit = (celsius * (9.0 / 5.0)) + 32;
        _logger.LogInformation("conversion requested");
        return farenheit;
    }

    /// <summary>
    /// Given a temperature in Celsius map it to an appropriate description.
    /// </summary>
    /// <param name="temperatureC">The temperature in Celsius.</param>
    /// <returns>The temperaature description.</returns>
    private static string MapFeelToTemperature(int temperatureC)
    {
        if (temperatureC <= 0)
        {
            return _summaries.First();
        }

        int summariesIndex = (temperatureC / 5) + 1;
        return summariesIndex >= _summaries.Length ? _summaries.Last() : _summaries[summariesIndex];
    }
}
