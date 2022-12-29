using Microsoft.Extensions.Logging.Abstractions;

using Uqs.Weather.Controllers;

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
}
