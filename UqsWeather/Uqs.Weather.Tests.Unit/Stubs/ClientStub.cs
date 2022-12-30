using AdamTibi.OpenWeather;

namespace Uqs.Weather.Tests.Unit.Stubs;

/// <summary>
/// A stub for dependency injection of IClient.
/// </summary>
public class ClientStub : IClient
{
    private readonly DateTime _now;
    private readonly IEnumerable<double> _sevenDaysTemps;

    public Units LastUnitSpy { get; set; }

    /// <summary>
    /// Construct a new <see cref="ClientStub"/>.
    /// </summary>
    /// <param name="now">The date to use.</param>
    /// <param name="sevenDaysTemps">List of seven days of temperatures to use.</param>
    public ClientStub(DateTime now, IEnumerable<double> sevenDaysTemps)
    {
        _now = now;
        _sevenDaysTemps = sevenDaysTemps;
    }

    /// <summary>
    /// Get the weather for the given latitude and longitude.
    /// </summary>
    /// <param name="latitude">The latititude.</param>
    /// <param name="longitude">The longitude.</param>
    /// <param name="excludes">The items to exclude.</param>
    /// <param name="unit">The units to use.</param>
    /// <returns>The response with the weather information.</returns>
    public Task<OneCallResponse> OneCallAsync(decimal latitude, decimal longitude, IEnumerable<Excludes> excludes,
                                              Units unit)
    {
        LastUnitSpy = unit;
        const int Days = 7;

        OneCallResponse response = new()
        {
            Daily = new Daily[Days]
        };

        for (int i = 0; i < Days; i++)
        {
            response.Daily[i] = new()
            {
                Dt = _now.AddDays(i),
                Temp = new()
                {
                    Day = _sevenDaysTemps.ElementAt(i)
                }
            };
        }

        return Task.FromResult(response);
    }
}
