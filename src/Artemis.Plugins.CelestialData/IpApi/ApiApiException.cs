using System;
using Artemis.Core;

namespace Artemis.Plugins.CelestialData.IpApi;

public class ApiApiException : ArtemisPluginException
{
    public IpData? Response { get; }

    public ApiApiException(string message, IpData? response) : base(message)
    {
        Response = response;
    }

    public ApiApiException(string message, IpData? response, Exception inner) : base(message, inner)
    {
    }
}