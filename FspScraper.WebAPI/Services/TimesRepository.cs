using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FspScraper.Common.Models;
using FspScraper.Common.Models.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FspScraper.WebAPI.Services
{
    public class TimesRepository : ITimesRepository
    {
        private readonly FspTimesContext _timesContext;
        private readonly ILogger<TimesRepository> _logger;
        public TimesRepository(FspTimesContext timesContext, ILogger<TimesRepository> logger)
        {
            _timesContext = timesContext;
            _logger = logger;
        }

        public async Task<FspTimes[]> GetTimes()
        {
            _logger.LogInformation($"Attempting to get times");
            var times = _timesContext.Times;
            return await times.ToArrayAsync();
        }

        public async Task<FspTimes> GetTimesByReg(string registrationNum)
        {
            _logger.LogInformation($"Attempting to get times for: {registrationNum}");
            var times = _timesContext.Times.
                            Where(a => a.RegistrationNum == registrationNum);
            return await times.FirstOrDefaultAsync();
        }
    }
}