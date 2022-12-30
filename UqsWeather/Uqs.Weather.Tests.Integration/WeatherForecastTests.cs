using System.Net.Http.Json;
namespace Uqs.Weather.Tests.Integration;

public class WeatherForecastTests
{
    private const string BaseAddress = "https://localhost:7283";
    private const string ApiUri = "/WeatherForecast/GetRealWeatherForecast";

    private record WeatherForecast(DateOnly Date, int TemperatureC, int TemperatureF, string? Summary);

    [Fact]
    public async Task GetRealWeatherForecast_Execute_GetNextFiveDays()
    {
        // Arrange
        HttpClient httpClient = new()
        {
            BaseAddress = new Uri(BaseAddress)
        };
        DateOnly today = DateOnly.FromDateTime(DateTime.Now.Date);
        DateOnly[] nextFiveDays = new[]
        {
            today.AddDays(1), today.AddDays(2), today.AddDays(3), today.AddDays(4), today.AddDays(5)
        };

        // Act
        HttpResponseMessage httpResponse = await httpClient.GetAsync(ApiUri);

        // Assert
        WeatherForecast[] weatherForecasts
            = await httpResponse.Content.ReadFromJsonAsync<WeatherForecast[]>() ?? throw new FormatException();
        for (int i = 0; i < 5; i++)
        {
            Assert.Equal(nextFiveDays[i], weatherForecasts[i].Date);
        }
    }
}
