using AdamTibi.OpenWeather;

using Microsoft.Extensions.Logging.Abstractions;

using NSubstitute;

using Uqs.Weather.Controllers;
using Uqs.Weather.Tests.Unit.Stubs;

namespace Uqs.Weather.Tests.Unit;

public class WeatherForecastControllerTests
{
    [Theory]
    [InlineData(0.0, 32.0)]
    [InlineData(-100.0, -148.0)]
    [InlineData(-10.1, 13.8)]
    [InlineData(10.0, 50.0)]
    public void ConvertCToF_Celsius_Farenheit(double centigrade, double farenheit)
    {
        // Arrange
        NullLogger<WeatherForecastController> logger = NullLogger<WeatherForecastController>.Instance;
        WeatherForecastController weatherForecastController = new(null!, null!, null!, logger);

        // Act
        double actual = weatherForecastController.ConvertCToF(centigrade);

        // Assert
        Assert.Equal(farenheit, actual, 1);
    }

    [Fact]
    public async void GetReal_NotInterestedInTodayWeather_WeatherForecastStartsFromNextDay()
    {
        // Arrange
        const double nextDayTemp = 3.3;
        const double dayFiveTemp = 7.7;
        DateTime today = new(2022, 1, 1);
        double[] realWeatherTemps = new[] { 2.0, nextDayTemp, 4.0, 5.5, 6.0, dayFiveTemp, 8.0 };
        //ClientStub clientStub = new(today, realWeatherTemps);
        IClient clientMock = Substitute.For<IClient>();
        _ = clientMock.OneCallAsync(Arg.Any<decimal>(),
                                    Arg.Any<decimal>(),
                                    Arg.Any<IEnumerable<Excludes>>(),
                                    Arg.Any<Units>()).Returns(x =>
        {
            //LastUnitSpy = unit;
            const int Days = 7;

            OneCallResponse response = new()
            {
                Daily = new Daily[Days]
            };

            for (int i = 0; i < Days; i++)
            {
                response.Daily[i] = new()
                {
                    Dt = today.AddDays(i),
                    Temp = new()
                    {
                        Day = realWeatherTemps.ElementAt(i)
                    }
                };
            }

            return Task.FromResult(response);
        });
        WeatherForecastController weatherForecastController = new(null!, null!, clientMock, null!);

        // Act
        IEnumerable<WeatherForecast> weatherForecasts = await weatherForecastController.GetReal();

        // Assert
        Assert.Equal(3, weatherForecasts.First().TemperatureC);
    }

    [Fact]
    public async void GetReal_DaysForForecastStartingNextDay_WeatherForecastFifthDayIsRealWeatherSixthDay()
    {
        // Arrange
        const double nextDayTemp = 3.3;
        const double dayFiveTemp = 7.7;
        DateTime today = new(2022, 1, 1);
        double[] realWeatherTemps = new[] { 2.0, nextDayTemp, 4.0, 5.5, 6.0, dayFiveTemp, 8.0 };
        ClientStub clientStub = new(today, realWeatherTemps);
        WeatherForecastController weatherForecastController = new(null!, null!, clientStub, null!);

        // Act
        IEnumerable<WeatherForecast> weatherForecasts = await weatherForecastController.GetReal();

        // Assert
        Assert.Equal(8, weatherForecasts.Last().TemperatureC);
    }

    [Fact]
    public async void GetReal_ForecastingForFiveDaysOnly_WeatherForecastHasFiveDays()
    {
        // Arrange
        const double nextDayTemp = 3.3;
        const double dayFiveTemp = 7.7;
        DateTime today = new(2022, 1, 1);
        double[] realWeatherTemps = new[] { 2.0, nextDayTemp, 4.0, 5.5, 6.0, dayFiveTemp, 8.0 };
        ClientStub clientStub = new(today, realWeatherTemps);
        WeatherForecastController weatherForecastController = new(null!, null!, clientStub, null!);

        // Act
        IEnumerable<WeatherForecast> weatherForecasts = await weatherForecastController.GetReal();

        // Assert
        Assert.Equal(5, weatherForecasts.Count());
    }

    [Fact]
    public async void GetReal_WeatherForecastDoesntConsiderDecimal_RealWeatherTempRoundedProperly()
    {
        // Arrange
        const double nextDayTemp = 3.3;
        const double dayFiveTemp = 7.7;
        DateTime today = new(2022, 1, 1);
        double[] realWeatherTemps = new[] { 2.0, nextDayTemp, 4.0, 5.5, 6.0, dayFiveTemp, 8.0 };
        ClientStub clientStub = new(today, realWeatherTemps);
        WeatherForecastController weatherForecastController = new(null!, null!, clientStub, null!);

        // Act
        IEnumerable<WeatherForecast> weatherForecasts = await weatherForecastController.GetReal();

        // Assert
        Assert.Equal(3, weatherForecasts.First().TemperatureC);
        Assert.Equal(8, weatherForecasts.Last().TemperatureC);
    }

    [Fact]
    public async void GetReal_TodayWeatherAndSixDaysForecastRecieved_RealDateMatchesNextDay()
    {
        // Arrange
        const double nextDayTemp = 3.3;
        const double dayFiveTemp = 7.7;
        DateTime today = new(2022, 1, 1);
        double[] realWeatherTemps = new[] { 2.0, nextDayTemp, 4.0, 5.5, 6.0, dayFiveTemp, 8.0 };
        ClientStub clientStub = new(today, realWeatherTemps);
        WeatherForecastController weatherForecastController = new(null!, null!, clientStub, null!);

        // Act
        IEnumerable<WeatherForecast> weatherForecasts = await weatherForecastController.GetReal();

        // Assert
        Assert.Equal(new DateOnly(2022, 1, 2), weatherForecasts.First().Date);
    }

    [Fact]
    public async void GetReal_TodayWeatherAndSixDaysForecastRecieved_RealDateMatchesLasttDay()
    {
        // Arrange
        const double nextDayTemp = 3.3;
        const double dayFiveTemp = 7.7;
        DateTime today = new(2022, 1, 1);
        double[] realWeatherTemps = new[] { 2.0, nextDayTemp, 4.0, 5.5, 6.0, dayFiveTemp, 8.0 };
        ClientStub clientStub = new(today, realWeatherTemps);
        WeatherForecastController weatherForecastController = new(null!, null!, clientStub, null!);

        // Act
        IEnumerable<WeatherForecast> weatherForecasts = await weatherForecastController.GetReal();

        // Assert
        Assert.Equal(new DateOnly(2022, 1, 6), weatherForecasts.Last().Date);
    }

    [Fact]
    public async void GetReal_RequestsToOpenWeather_MetricUnitIsUsed()
    {
        // Arrange
        const double nextDayTemp = 3.3;
        const double dayFiveTemp = 7.7;
        DateTime today = new(2022, 1, 1);
        double[] realWeatherTemps = new[] { 2.0, nextDayTemp, 4.0, 5.5, 6.0, dayFiveTemp, 8.0 };
        //ClientStub clientStub = new(today, realWeatherTemps);
        IClient clientMock = Substitute.For<IClient>();
        _ = clientMock.OneCallAsync(Arg.Any<decimal>(),
                                    Arg.Any<decimal>(),
                                    Arg.Any<IEnumerable<Excludes>>(),
                                    Arg.Any<Units>()).Returns(x =>
                                    {
                                        //LastUnitSpy = unit;
                                        const int Days = 7;

                                        OneCallResponse response = new()
                                        {
                                            Daily = new Daily[Days]
                                        };

                                        for (int i = 0; i < Days; i++)
                                        {
                                            response.Daily[i] = new()
                                            {
                                                Dt = today.AddDays(i),
                                                Temp = new()
                                                {
                                                    Day = realWeatherTemps.ElementAt(i)
                                                }
                                            };
                                        }

                                        return Task.FromResult(response);
                                    });
        WeatherForecastController weatherForecastController = new(null!, null!, clientMock, null!);

        // Act
        _ = await weatherForecastController.GetReal();

        // Assert
        _ = await clientMock.Received().OneCallAsync(Arg.Any<decimal>(),
                                                     Arg.Any<decimal>(),
                                                     Arg.Any<IEnumerable<Excludes>>(),
                                                     Arg.Is<Units>(u => u == Units.Metric));
    }
}
