using System.Collections.Generic;
using FspScraper.Common.Models;

namespace FspScraper.Common.Interfaces
{
    public interface ITimesParser
    {
        FspTimes ParseTimes(List<string> labels, List<string> values);
    }
}