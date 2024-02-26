using System;
using System.Collections.Generic;
using System.Linq;
using Artemis.Core.Modules;
using CoordinateSharp;

namespace Artemis.Plugins.CelestialData;

public class CelestialDataModel : DataModel
{
    private int _currentDay = -1;
    private int _currentHour = -1;
    private double _dayPeekAltitude = 91;
    private Dictionary<int, double> _currentHourNoonValues = new();
    private Dictionary<int, double> _currentHourZenithValues = new();

    [DataModelProperty(Name = "Solar Noon Percentage", Description = "Value in 0-100. 100 means peak brightness of the day")]
    public double SolarNoonPercentage
    {
        get
        {
            var dateTime = DateTime.UtcNow;

            if (dateTime.Day != _currentDay)
            {
                _dayPeekAltitude = PeakSunAngle(dateTime);
            }

            if (dateTime.Hour != _currentHour)
            {
                _currentDay = dateTime.Day;
                _currentHour = dateTime.Hour;
                GenerateHourData(dateTime);
            }

            return _currentHourNoonValues[dateTime.Minute];
        }
    }

    [DataModelProperty(Name = "Zenith Percentage", Description = "Value in 0-100. 100 means sun is directly above you")]
    public double ZenithPercentage
    {
        get
        {
            var dateTime = DateTime.UtcNow;

            if (dateTime.Hour != _currentHour)
            {
                _currentHour = dateTime.Hour;
                GenerateHourData(dateTime);
            }

            return _currentHourZenithValues[dateTime.Minute];
        }
    }

    [DataModelProperty(Name = "Day Peak Altitude (Degrees)", Description = "Maximum Altitude of the sun during the day")]
    public double DayPeakAltitude
    {
        get
        {
            if (_dayPeekAltitude > 90)
            {
                _dayPeekAltitude = PeakSunAngle(DateTime.UtcNow);
            }
            return _dayPeekAltitude;
        }
    }

    private void GenerateHourData(DateTime date)
    {
        _currentHourZenithValues = new Dictionary<int, double>();
        _currentHourNoonValues = new Dictionary<int, double>();

        var peakBrightness = BrightnessFunction(_dayPeekAltitude);

        date = date.AddMinutes(-date.Minute);
        for (var i = 0; i < 60; i++)
        {
            CelestialModule.Coordinate.GeoDate = date;
            var noonPercentage = BrightnessFunction(CelestialModule.Coordinate.CelestialInfo.SunAltitude);
            var clampedNoonP = Math.Clamp(noonPercentage, 0d, 100d);
            _currentHourZenithValues[i] = clampedNoonP;
            _currentHourNoonValues[i] = clampedNoonP * 100 / peakBrightness;
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

    private double PeakSunAngle(DateTime date)
    {
        date.Deconstruct(out var year, out var month, out var day);

        var noon = new DateTime(year, month, day, 12, 30, 0, DateTimeKind.Utc);
        CelestialModule.Coordinate.GeoDate = noon;
        var noonAlt = CelestialModule.Coordinate.CelestialInfo.SunAltitude;
        
        var laterNoon = new DateTime(year, month, day, 12, 31, 0, DateTimeKind.Utc);
        CelestialModule.Coordinate.GeoDate = laterNoon;
        var laterAlt = CelestialModule.Coordinate.CelestialInfo.SunAltitude;

        var increment = laterAlt > noonAlt ? 1 : -1;

        var highestAlt = 0.0;

        while (true)
        {
            noon = noon.AddMinutes(increment);
            CelestialModule.Coordinate.GeoDate = noon;

            var newAlt = CelestialModule.Coordinate.CelestialInfo.SunAltitude;
            if (newAlt > highestAlt)
            {
                highestAlt = newAlt;
            }
            else
            {
                return highestAlt;
            }
        }
    }
}