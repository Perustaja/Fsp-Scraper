using System.Linq;
using System.Collections.Generic;
using FspScraper.Common.Models;
using FspScraper.Common.Interfaces;

namespace FspScraper.Scraper
{
    public class FspTimesParser : ITimesParser
    {
        public FspTimes ParseTimes(List<string> labels, List<string> values)
        {
            var dict = new Dictionary<string, decimal>();
            // starts at 1 to avoid parsing the N number
            for (var i = 1; i < labels.Count(); i++)
            {
                // if value length is over 8 (i.e more than the max possible amount of numbers 99999.99)
                // then creates a substring without any potential text after the time value
                // then, parses the string safely into a decimal since it will always be a number
                if (values[i].Length > 8)
                {
                    values[i] = values[i].Substring(0, values[i].IndexOf(" "));
                }
                dict.Add(labels[i], decimal.Parse(values[i]));
            };

            // create a model instance from the dictionary
            decimal value;
            var times = new FspTimes()
            {
                RegistrationNum = values[0].Substring(0, values[0].IndexOf(" ")),
                Hobbs = dict.TryGetValue("Hobbs", out value) ? decimal.Round(value,2) : default(decimal),
                AirTime = dict.TryGetValue("Air Time", out value) ? (int)decimal.Round(value,2) : default(int),
                Tach1 = dict.TryGetValue("Tach 1", out value) ? decimal.Round(value,2) : default(decimal),
                Tach2 = dict.TryGetValue("Tach 2", out value) ? decimal.Round(value,2) : default(decimal),
                Prop1 = dict.TryGetValue("Prop 1", out value) ? decimal.Round(value,2) : default(decimal),
                Prop2 = dict.TryGetValue("Prop 2", out value) ? decimal.Round(value,2) : default(decimal),
                AircraftTotal = dict.TryGetValue("Aircraft Total Time", out value) ? decimal.Round(value,2) : default(decimal),
                Engine1Total = dict.TryGetValue("Engine 1 Current Time", out value) ? decimal.Round(value,2) : default(decimal),
                Engine2Total = dict.TryGetValue("Engine 2 Current Time", out value) ? decimal.Round(value,2) : default(decimal),
                Cycles = dict.TryGetValue("Cycles", out value) ? (int)decimal.Round(value,2) : default(int)
            };

            return times;
        }
    }
}