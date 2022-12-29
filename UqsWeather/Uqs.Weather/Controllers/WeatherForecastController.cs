using AdamTibi.OpenWeather;

using Microsoft.AspNetCore.Mvc;

using Uqs.Weather.Wrappers;

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
    private readonly IRandomWrapper _randomWrapper;
    private readonly INowWrapper _nowWrapper;
    private readonly IClient _openWeatherClient;
    private readonly ILogger<WeatherForecastController> _logger;

    public WeatherForecastController(IRandomWrapper randomWrapper, INowWrapper nowWrapper,
                                     IClient openWeatherClient,
                                     ILogger<WeatherForecastController> logger)
    {
        _randomWrapper = randomWrapper;
        _nowWrapper = nowWrapper;
        _openWeatherClient = openWeatherClient;
        _logger = logger;
    }

    [HttpGet(Name = "GetWeatherForecast")]
    public IEnumerable<WeatherForecast> Get() => Enumerable.Range(1, 5).Select(index => new WeatherForecast
    {
        Date = DateOnly.FromDateTime(_nowWrapper.Now.AddDays(index)),
        TemperatureC = _randomWrapper.Next(-20, 55),
        Summary = _summaries[_randomWrapper.Next(0, _summaries.Length)]
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
                Date = DateOnly.FromDateTime(_nowWrapper.Now).AddDays(i + 1),
                TemperatureC = _randomWrapper.Next(-0, 55)
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
        //string apiKey = _configuration["OpenWeather:Key"]
        //                ?? throw new KeyNotFoundException("Configurtion for OpenWeather:Key not found.");
        //HttpClient httpClient = new();
        OneCallResponse oneCallResponse
            = await _openWeatherClient
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
