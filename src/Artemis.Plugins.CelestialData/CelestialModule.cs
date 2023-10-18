using System.Collections.Generic;
using Artemis.Core;
using Artemis.Core.Modules;
using Artemis.Plugins.CelestialData.IpApi;

namespace Artemis.Plugins.CelestialData;

[PluginFeature(Name = "Celestial Data")]
public class CelestialModule : Module<CelestialDataModel>
{
    public static double Lat = -1;
    public static double Lon = -1;
    
    private readonly PluginSettings _pluginSettings;

    public CelestialModule(PluginSettings pluginSettings)
    {
        _pluginSettings = pluginSettings;
    }

    public override void Enable()
    {
        var lat = _pluginSettings.GetSetting("lat", -1d);
        var lon = _pluginSettings.GetSetting("lon", -1d);
        
        if (lat.Value <= 0)
        {
            var ipData = IpApiClient.GetIpData().Result;
            if (ipData == null)
            {
                return;
            }
            lat.Value = ipData.Lat;
            lon.Value = ipData.Lon;
            lat.Save();
            lon.Save();
        }
        else
        {
            Lat = lat.Value;
            Lon = lon.Value;
        }
    }

    public override void Disable()
    {
    }

    public override void Update(double deltaTime)
    {
    }

    public override List<IModuleActivationRequirement>? ActivationRequirements => null;
}