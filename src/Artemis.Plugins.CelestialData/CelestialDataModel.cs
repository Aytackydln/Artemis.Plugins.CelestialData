using System;
using System.Collections.Generic;
using Artemis.Core.Modules;
using CoordinateSharp;

namespace Artemis.Plugins.CelestialData;

public class CelestialDataModel : DataModel
{
    private int _currentHour = -1;
    private Dictionary<int, double> _currentHourValues = new();

    [DataModelProperty(Name = "Solar Noon Percentage",
        Description = "Value in 0-100. 100 means sun is directly above you")]
    public double SolarNoonPercentage
    {
        get {
            var d = DateTime.Now;

            if (d.Hour == _currentHour) return _currentHourValues[d.Minute];
    
            _currentHour = d.Hour;
            _currentHourValues = new Dictionary<int, double>();

            d = d.AddMinutes(-d.Minute);
            for (var i = 0; i < 60; i++)
            {
                var c = new Coordinate(CelestialModule.Lat, CelestialModule.Lon, d);
                var b = c.CelestialInfo.SunAltitude * 100d / 90;
                _currentHourValues[i] = Math.Clamp(b, 0d, 100d);
                d = d.AddMinutes(1);
            }

            return _currentHourValues[d.Minute];
        }
    }
}