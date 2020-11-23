using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FspScraper.Common.Interfaces;
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

        public async Task<List<FspTimes>> GetTimes() => await _timesContext.Set<FspTimes>().ToListAsync();

        public async Task<FspTimes> GetTimesByReg(string registrationNum) 
            => await _timesContext.Set<FspTimes>().Where(t => t.RegistrationNum.ToLower() == registrationNum.ToLower()).FirstOrDefaultAsync();

    }
}