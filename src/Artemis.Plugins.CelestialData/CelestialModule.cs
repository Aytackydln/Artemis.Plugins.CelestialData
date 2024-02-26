using System;
using System.Collections.Generic;
using Artemis.Core;
using Artemis.Core.Modules;
using Artemis.Plugins.CelestialData.IpApi;
using CoordinateSharp;
using JetBrains.Annotations;

namespace Artemis.Plugins.CelestialData;

[PluginFeature(Name = "Celestial Data")]
[UsedImplicitly]
public class CelestialModule(PluginSettings pluginSettings) : Module<CelestialDataModel>
{
    private static double Lat { get; set; } = -1;
    private static double Lon { get; set; } = -1;

    private static readonly EagerLoad El = new(EagerLoadType.Celestial)
    {
        Extensions = new EagerLoad_Extensions(EagerLoad_ExtensionsType.Solar_Cycle)
    };
    public static Coordinate Coordinate { get; private set; } = new(El);

    public override void Enable()
    {
        var lat = pluginSettings.GetSetting("lat", -1d);
        var lon = pluginSettings.GetSetting("lon", -1d);

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
        Lat = lat.Value;
        Lon = lon.Value;

        Coordinate = new Coordinate(Lat, Lon, DateTime.Now, El);
    }

    public override void Disable()
    {
    }

    public override void Update(double deltaTime)
    {
    }

    public override List<IModuleActivationRequirement>? ActivationRequirements => null;
}