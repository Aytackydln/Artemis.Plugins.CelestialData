using Newtonsoft.Json;

namespace Artemis.Plugins.CelestialData.IpApi;

public class IpData
{
    public double Lat { get; }
    public double Lon { get; }

    [JsonConstructor]
    public IpData(double lat, double lon)
    {
        Lat = lat;
        Lon = lon;
    }
}