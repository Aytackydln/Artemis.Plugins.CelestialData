using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Artemis.Plugins.CelestialData.IpApi;

public static class IpApiClient
{
    public static async Task<IpData?> GetIpData()
    {
        using HttpClient client = new();
        return await client.GetFromJsonAsync<IpData>("http://ip-api.com/json");
    }
}