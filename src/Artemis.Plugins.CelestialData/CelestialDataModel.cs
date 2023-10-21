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
        get
        {
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
            var coordinate = new Coordinate(52.4928, 13.4039, date);
            var noonPercentage = BrightnessFunction(coordinate.CelestialInfo.SunAltitude);
            _currentHourValues[i] = Math.Clamp(noonPercentage, 0d, 100d);
            date = date.AddMinutes(1);
        }
    }

    private static double BrightnessFunction(double x)
    {
        const double noonAngle = 90;        //x2
        const double darknessAngle = -18;   //x1
        //linear plotting   (y2 - y1)(x2 - x1)
        const double m = (100 - 0) / (noonAngle - darknessAngle);
        
        // y = m(x - x1) + y1
        return m * (x - darknessAngle) + 0;
    }
}