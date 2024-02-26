namespace Artemis.Plugins.CelestialData.IpApi;

public class IpData(double lat, double lon, string status, string? message)
{
    public double Lat { get; } = lat;
    public double Lon { get; } = lon;

    public string Status { get; } = status;
    public string? Message { get; } = message;
}