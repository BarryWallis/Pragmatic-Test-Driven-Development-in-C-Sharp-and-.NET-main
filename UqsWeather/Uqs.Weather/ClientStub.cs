using AdamTibi.OpenWeather;

namespace Uqs.Weather;

/// <summary>
/// A stub for dependency injection of IClient.
/// </summary>
public class ClientStub : IClient
{
    public Task<OneCallResponse> OneCallAsync(decimal latitude, decimal longitude, IEnumerable<Excludes> excludes, Units unit)
    {
        const int Days = 7;
        OneCallResponse response = new()
        {
            Daily = new Daily[Days]
        };

        for (int i = 0; i < Days; i++)
        {
            response.Daily[i] = new()
            {
                Dt = DateTime.Now.AddDays(i),
                Temp = new()
            };
            response.Daily[i].Temp.Day = Random.Shared.Next(-20, 55);
        }

        return Task.FromResult(response);
    }
}
