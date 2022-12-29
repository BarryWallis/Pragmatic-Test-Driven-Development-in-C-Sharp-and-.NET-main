// See https://aka.ms/new-console-template for more information

using Microsoft.Extensions.Logging.Abstractions;

using Uqs.Weather.Controllers;

NullLogger<WeatherForecastController> logger = NullLogger<WeatherForecastController>.Instance;
WeatherForecastController weatherForecastController = new(logger, null!);
double farenheit1 = weatherForecastController.ConvertCToF(-1.0);
if (farenheit1 != 30.20)
{
    throw new Exception("Invalid");
}

double farenheit2 = weatherForecastController.ConvertCToF(1.2);
if (farenheit2 != 34.16)
{
    throw new Exception("Invalid");
}

Console.WriteLine("Test passed");
