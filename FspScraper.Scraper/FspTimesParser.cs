using System.Linq;
using System.Collections.Generic;
using FspScraper.Common.Models;
using FspScraper.Common.Interfaces;
using FspScraper.Common;

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
                decimal val;
                decimal.TryParse(values[i], out val);
                dict.Add(labels[i], val);
            };

            // create a model instance from the dictionary
            decimal value;
            // registration numbers may be simulators
            var regNum = new string(values[0].SkipWhile(c => c != 'N').Take(6).ToArray());
            if (string.IsNullOrWhiteSpace(regNum))
                regNum = values[0].Substring(10);
            var times = new FspTimes()
            {
                RegistrationNum = regNum,
                Hobbs = dict.TryGetValue(LabelConstants.HOBBS, out value) ? decimal.Round(value,2) : default(decimal),
                AirTime = dict.TryGetValue(LabelConstants.AIRTIME, out value) ? (int)decimal.Round(value,2) : default(int),
                Prop1Total = dict.TryGetValue(LabelConstants.PROP1, out value) ? decimal.Round(value,2) : default(decimal),
                Prop2Total = dict.TryGetValue(LabelConstants.PROP2, out value) ? decimal.Round(value,2) : default(decimal),
                Engine1Total = dict.TryGetValue(LabelConstants.ENG1, out value) ? decimal.Round(value,2) : default(decimal),
                Engine2Total = dict.TryGetValue(LabelConstants.ENG2, out value) ? decimal.Round(value,2) : default(decimal),
            };

            return times;
        }
    }
}