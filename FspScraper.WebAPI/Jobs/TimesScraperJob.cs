using System;
using System.Linq;
using System.Threading.Tasks;
using FspScraper.Common.Models.Contexts;
using FspScraper.Scraper;
using Hangfire;
using Microsoft.Extensions.Logging;

namespace FspScraper.WebAPI.Jobs
{
    public class TimesScraperJob : ITimesScraperJob
    {
        private readonly ILogger _logger;
        private readonly FspTimesContext _timesContext;
        public TimesScraperJob(FspTimesContext context, ILogger<TimesScraperJob> logger)
        {
            _timesContext = context;
            _logger = logger;
        }
        public async Task Run(IJobCancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            await UpdateTimes();
        }
        public async Task UpdateTimes()
        {
            _logger.LogInformation($"{DateTime.Now}: Attempting to start scraping background service.");
            var scraper = new FspTimesScraper();
            var times = scraper.Run();
            foreach (var set in times)
            {
                if (_timesContext.Times.Any(e => e.RegistrationNum == set.RegistrationNum))
                    _timesContext.Update(set);
                else 
                    _timesContext.Add(set);
            }
            await _timesContext.SaveChangesAsync();
            _logger.LogInformation($"{DateTime.Now}: Scraping service finished.");
        }
    }
}