using System.Collections.Generic;
using System.Threading.Tasks;
using FspScraper.Common.Models;

namespace FspScraper.Common.Interfaces
{
    public interface ITimesRepository
    {
        // Returns all aicraft times
        Task<List<FspTimes>> GetTimes();
        // Returns one set of aircraft times where the registration number equals the passed argument
        Task<FspTimes> GetTimesByReg(string registrationNum);
    }
}