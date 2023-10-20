using System;
using System.Collections.Generic;
using System.Linq;
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
            var dateTime = DateTime.Now;

            if (dateTime.Hour != _currentHour)
            {
                _currentHour = dateTime.Hour;
                GenerateHourData(dateTime);
            }

            return _currentHourValues[dateTime.Minute];
        }
    }

    private void GenerateHourData(DateTime date)
    {
        _currentHourValues = new Dictionary<int, double>();
        
        date = date.AddMinutes(-date.Minute);
        for (var i = 0; i < 60; i++)
        {
            var coordinate = new Coordinate(CelestialModule.Lat, CelestialModule.Lon, date);
            var noonPercentage = coordinate.CelestialInfo.SunAltitude * 100d / 90;
            _currentHourValues[i] = Math.Clamp(noonPercentage, 0d, 100d);
            date = date.AddMinutes(1);
        }
    }
}