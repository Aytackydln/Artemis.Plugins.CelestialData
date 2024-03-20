using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace Artemis.Plugins.CelestialData.IpApi;

public static class IpApiClient
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        TypeInfoResolverChain = { IpApiSourceGenerationContext.Default },
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };
    
    public static async Task<IpData?> GetIpData()
    {
        using HttpClient client = new();
        var response = await client.GetFromJsonAsync<IpData>("http://ip-api.com/json?fields=status,message,lat,lon", JsonOptions);

        if (response == null)
        {
            throw new ApiApiException("IpApi returned empty response", response);
        }
        if (response.Status != "success")
        {
            throw new ApiApiException("IpApi returned error: " + response.Message, response);
        }
        
        return response;
    }
}