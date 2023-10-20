using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Artemis.Core;

namespace Artemis.Plugins.CelestialData.IpApi;

public static class IpApiClient
{
    public static async Task<IpData?> GetIpData()
    {
        using HttpClient client = new();
        var response = await client.GetFromJsonAsync<IpData>("http://ip-api.com/json?fields=status,message,lat,lon");

        if (response == null)
        {
            throw new ArtemisPluginException("IpApi returned empty response");
        }
        if (response.Status != "success")
        {
            throw new ArtemisPluginException("IpApi returned error: " + response.Message);
        }
        
        return response;
    }
}